using Admission.Auth.Api.Contracts;
using Admission.Auth.Common;
using Admission.Auth.Domain.Entities;
using Admission.Auth.Domain.Enums;
using Admission.Auth.Messaging;
using Admission.Auth.Options;
using Admission.Auth.Persistence;
using Admission.Auth.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Admission.Auth.Application;

public sealed class AuthService(
    AuthDbContext dbContext,
    IPasswordHasher<AuthUser> passwordHasher,
    IPasswordPolicyValidator passwordPolicyValidator,
    IAccessTokenFactory accessTokenFactory,
    IMailEventPublisher mailEventPublisher,
    IRandomTokenGenerator randomTokenGenerator,
    ITokenHasher tokenHasher,
    IOptions<AuthOptions> authOptions,
    IOptions<SecurityOptions> securityOptions,
    IOptions<AuthDebugOptions> authDebugOptions,
    ILogger<AuthService> logger)
{
    private readonly AuthOptions _authOptions = authOptions.Value;
    private readonly SecurityOptions _securityOptions = securityOptions.Value;
    private readonly AuthDebugOptions _authDebugOptions = authDebugOptions.Value;

    public async Task<RegisterResponse> RegisterApplicantAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = NormalizeEmail(request.Email);
        await EnsureEmailIsUniqueAsync(normalizedEmail, cancellationToken);

        EnsurePasswordIsValid(request.Password);

        var user = new AuthUser
        {
            Email = request.Email.Trim(),
            NormalizedEmail = normalizedEmail,
            Role = UserRole.Applicant,
            EmailConfirmed = false
        };

        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        dbContext.Users.Add(user);

        var token = await CreateActionTokenAsync(user.Id, ActionTokenType.EmailConfirmation, cancellationToken);
        await SaveAuthAndOutboxChangesAsync(
            async () =>
            {
                await mailEventPublisher.PublishEmailConfirmationAsync(user.Email, token, cancellationToken);
            }, cancellationToken);

        logger.LogInformation("Applicant user registered: {UserId}", user.Id);
        return new RegisterResponse(user.Id, user.Role.ToString(), "User registered successfully");
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, string? userAgent, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var user = await GetByEmailAsync(request.Email, cancellationToken);

        if (user is null || !user.IsActive)
        {
            throw new AppException("Invalid credentials.", StatusCodes.Status401Unauthorized);
        }

        var passwordVerification = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (passwordVerification == PasswordVerificationResult.Failed)
        {
            throw new AppException("Invalid credentials.", StatusCodes.Status401Unauthorized);
        }

        var tokenPair = await IssueTokenPairAsync(user, userAgent, ipAddress, cancellationToken);
        return new LoginResponse(
            tokenPair.AccessToken.Token,
            tokenPair.RefreshToken,
            tokenPair.AccessToken.ExpiresInSeconds,
            user.Role.ToString(),
            user.MustChangePassword);
    }

    public async Task<RefreshResponse> RefreshAsync(string refreshToken, string? userAgent, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var nowUtc = DateTimeOffset.UtcNow;
        var refreshSessionHash = tokenHasher.Hash(refreshToken);

        var refreshSession = await dbContext.RefreshSessions
            .Include(x => x.User)
            .SingleOrDefaultAsync(x => x.TokenHash == refreshSessionHash, cancellationToken);

        if (refreshSession is null || !refreshSession.IsActive(nowUtc))
        {
            throw new AppException("Invalid refresh token.", StatusCodes.Status401Unauthorized);
        }

        if (!refreshSession.User.IsActive || refreshSession.SecurityVersion != refreshSession.User.SecurityVersion)
        {
            throw new AppException("Invalid refresh token.", StatusCodes.Status401Unauthorized);
        }

        refreshSession.RevokedAt = nowUtc;

        var tokenPair = await IssueTokenPairAsync(refreshSession.User, userAgent, ipAddress, cancellationToken);
        var nextSession = await dbContext.RefreshSessions
            .SingleAsync(x => x.TokenHash == tokenHasher.Hash(tokenPair.RefreshToken), cancellationToken);
        refreshSession.ReplacedBySessionId = nextSession.Id;

        await dbContext.SaveChangesAsync(cancellationToken);

        return new RefreshResponse(tokenPair.AccessToken.Token, tokenPair.RefreshToken, tokenPair.AccessToken.ExpiresInSeconds);
    }

    public async Task LogoutAsync(Guid userId, string refreshToken, CancellationToken cancellationToken = default)
    {
        var refreshSessionHash = tokenHasher.Hash(refreshToken);
        var refreshSession = await dbContext.RefreshSessions.SingleOrDefaultAsync(x => x.TokenHash == refreshSessionHash, cancellationToken);

        if (refreshSession is null)
        {
            return;
        }

        if (refreshSession.UserId != userId)
        {
            throw new AppException("Refresh token does not belong to current user.", StatusCodes.Status403Forbidden);
        }

        refreshSession.RevokedAt ??= DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        var user = await GetRequiredUserAsync(userId, cancellationToken);
        var passwordVerification = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword);

        if (passwordVerification == PasswordVerificationResult.Failed)
        {
            throw new AppException("Current password is invalid.", StatusCodes.Status400BadRequest);
        }

        EnsurePasswordIsValid(request.NewPassword);

        user.PasswordHash = passwordHasher.HashPassword(user, request.NewPassword);
        user.MustChangePassword = false;
        user.SecurityVersion++;
        user.UpdatedAt = DateTimeOffset.UtcNow;

        await RevokeAllRefreshSessionsAsync(user.Id, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateMyEmailAsync(Guid userId, UpdateMyEmailRequest request, CancellationToken cancellationToken = default)
    {
        var user = await GetRequiredUserAsync(userId, cancellationToken);
        var normalizedEmail = NormalizeEmail(request.Email);

        if (!string.Equals(user.NormalizedEmail, normalizedEmail, StringComparison.Ordinal))
        {
            await EnsureEmailIsUniqueAsync(normalizedEmail, cancellationToken);
            user.Email = request.Email.Trim();
            user.NormalizedEmail = normalizedEmail;
            user.EmailConfirmed = false;
            user.UpdatedAt = DateTimeOffset.UtcNow;

            var token = await CreateActionTokenAsync(user.Id, ActionTokenType.EmailConfirmation, cancellationToken);
            await SaveAuthAndOutboxChangesAsync(
                () => mailEventPublisher.PublishEmailConfirmationAsync(user.Email, token, cancellationToken),
                cancellationToken);
        }
    }

    public async Task<MeResponse> GetMeAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await GetRequiredUserAsync(userId, cancellationToken);
        return new MeResponse(
            user.Id,
            user.Email,
            user.Role.ToString(),
            user.FacultyId,
            user.EmailConfirmed,
            user.MustChangePassword);
    }

    public async Task<MessageResponse> LogoutAllAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await GetRequiredUserAsync(userId, cancellationToken);
        user.SecurityVersion++;
        user.UpdatedAt = DateTimeOffset.UtcNow;

        await RevokeAllRefreshSessionsAsync(userId, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new MessageResponse("Logged out from all devices successfully");
    }

    public async Task<StaffCreateResponse> CreateStaffAsync(StaffCreateRequest request, CancellationToken cancellationToken = default)
    {
        var role = GetStaffRole(request.Role);
        EnsureStaffRole(role);
        EnsureFacultyRule(role, request.FacultyId);

        var normalizedEmail = NormalizeEmail(request.Email);
        await EnsureEmailIsUniqueAsync(normalizedEmail, cancellationToken);

        var temporaryPassword = GenerateTemporaryPassword();
        var user = new AuthUser
        {
            Email = request.Email.Trim(),
            NormalizedEmail = normalizedEmail,
            Role = role,
            FacultyId = request.FacultyId,
            EmailConfirmed = true,
            MustChangePassword = true
        };

        user.PasswordHash = passwordHasher.HashPassword(user, temporaryPassword);

        dbContext.Users.Add(user);
        await SaveAuthAndOutboxChangesAsync(
            () => mailEventPublisher.PublishManagerCreatedAsync(
                user.Email,
                user.Role.ToString(),
                temporaryPassword,
                cancellationToken),
            cancellationToken);

        return new StaffCreateResponse(
            user.Id,
            user.Role.ToString(),
            temporaryPassword,
            "Staff user created");
    }

    private static UserRole GetStaffRole(StaffCreationRoles role)
    {
        return role switch
        {
            StaffCreationRoles.GeneralManager => UserRole.GeneralManager,
            StaffCreationRoles.Manager => UserRole.Manager,
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
        };
    }
    
    public async Task<MessageResponse> DeleteStaffAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await GetRequiredUserAsync(userId, cancellationToken);
        EnsureStaffRole(user.Role);

        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new MessageResponse("Staff user deleted successfully");
    }

    public async Task<StaffListResponse> GetStaffListAsync(UserRole? role, int page, int size, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        size = Math.Clamp(size, 1, 100);

        var query = dbContext.Users.AsNoTracking()
            .Where(x => x.Role == UserRole.Manager || x.Role == UserRole.GeneralManager);

        if (role is not null)
        {
            var parsedRole = role.Value;
            EnsureStaffRole(parsedRole);
            query = query.Where(x => x.Role == parsedRole);
        }

        var count = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.Email)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(x => new StaffListItem(x.Id, x.Email, x.Role.ToString(), x.FacultyId))
            .ToListAsync(cancellationToken);

        return new StaffListResponse(items, new PaginationResponse(page, size, count));
    }

    public async Task<ActionTokenRequestedResponse> RequestEmailConfirmationAsync(TokenRequest request, CancellationToken cancellationToken = default)
    {
        var user = await GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            return new ActionTokenRequestedResponse("If account exists, confirmation token was generated.");
        }

        var token = await CreateActionTokenAsync(user.Id, ActionTokenType.EmailConfirmation, cancellationToken);
        await SaveAuthAndOutboxChangesAsync(
            () => mailEventPublisher.PublishEmailConfirmationAsync(user.Email, token, cancellationToken),
            cancellationToken);
        return new ActionTokenRequestedResponse(
            "If account exists, confirmation token was generated.",
            _authDebugOptions.ExposeActionTokens ? token : null);
    }

    public async Task<MessageResponse> ConfirmEmailAsync(TokenConfirmRequest request, CancellationToken cancellationToken = default)
    {
        return await ConfirmEmailAsync(request.Token, cancellationToken);
    }

    public async Task<MessageResponse> ConfirmEmailAsync(string token, CancellationToken cancellationToken = default)
    {
        var actionToken = await GetValidActionTokenAsync(token, ActionTokenType.EmailConfirmation, cancellationToken);
        var user = await GetRequiredUserAsync(actionToken.UserId, cancellationToken);

        actionToken.UsedAt = DateTimeOffset.UtcNow;
        user.EmailConfirmed = true;
        user.UpdatedAt = DateTimeOffset.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new MessageResponse("Email confirmed successfully");
    }

    public async Task<ActionTokenRequestedResponse> RequestPasswordResetAsync(TokenRequest request, CancellationToken cancellationToken = default)
    {
        var user = await GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            return new ActionTokenRequestedResponse("If account exists, reset token was generated.");
        }

        var token = await CreateActionTokenAsync(user.Id, ActionTokenType.PasswordReset, cancellationToken);
        await SaveAuthAndOutboxChangesAsync(
            () => mailEventPublisher.PublishPasswordResetAsync(user.Email, token, cancellationToken),
            cancellationToken);
        return new ActionTokenRequestedResponse(
            "If account exists, reset token was generated.",
            _authDebugOptions.ExposeActionTokens ? token : null);
    }

    public async Task<MessageResponse> ConfirmPasswordResetAsync(TokenConfirmRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.NewPassword))
        {
            throw new AppException("New password is required.", StatusCodes.Status400BadRequest);
        }

        EnsurePasswordIsValid(request.NewPassword);

        var actionToken = await GetValidActionTokenAsync(request.Token, ActionTokenType.PasswordReset, cancellationToken);
        var user = await GetRequiredUserAsync(actionToken.UserId, cancellationToken);

        user.PasswordHash = passwordHasher.HashPassword(user, request.NewPassword);
        user.MustChangePassword = false;
        user.SecurityVersion++;
        user.UpdatedAt = DateTimeOffset.UtcNow;
        actionToken.UsedAt = DateTimeOffset.UtcNow;

        await RevokeAllRefreshSessionsAsync(user.Id, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new MessageResponse("Password reset successfully");
    }

    private async Task<(IssuedAccessToken AccessToken, string RefreshToken)> IssueTokenPairAsync(
        AuthUser user,
        string? userAgent,
        string? ipAddress,
        CancellationToken cancellationToken)
    {
        var accessToken = accessTokenFactory.Create(user);
        var refreshToken = randomTokenGenerator.Generate(_authOptions.RefreshTokenLength);
        var nowUtc = DateTimeOffset.UtcNow;

        dbContext.RefreshSessions.Add(new RefreshSession
        {
            UserId = user.Id,
            TokenHash = tokenHasher.Hash(refreshToken),
            ExpiresAt = nowUtc.AddDays(_authOptions.RefreshTokenDays),
            SecurityVersion = user.SecurityVersion,
            UserAgent = userAgent,
            IpAddress = ipAddress
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        return (accessToken, refreshToken);
    }

    private async Task<string> CreateActionTokenAsync(Guid userId, ActionTokenType type, CancellationToken cancellationToken)
    {
        var nowUtc = DateTimeOffset.UtcNow;

        var staleTokens = await dbContext.UserActionTokens
            .Where(x => x.UserId == userId && x.Type == type && x.UsedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var staleToken in staleTokens)
        {
            staleToken.UsedAt = nowUtc;
        }

        var token = randomTokenGenerator.Generate();
        dbContext.UserActionTokens.Add(new UserActionToken
        {
            UserId = userId,
            Type = type,
            TokenHash = tokenHasher.Hash(token),
            ExpiresAt = nowUtc.AddMinutes(_securityOptions.ActionTokenLifetimeMinutes)
        });

        return token;
    }

    private async Task<UserActionToken> GetValidActionTokenAsync(string token, ActionTokenType type, CancellationToken cancellationToken)
    {
        var tokenHash = tokenHasher.Hash(token);
        var nowUtc = DateTimeOffset.UtcNow;

        var actionToken = await dbContext.UserActionTokens
            .SingleOrDefaultAsync(x => x.TokenHash == tokenHash && x.Type == type, cancellationToken);

        if (actionToken is null || !actionToken.IsUsable(nowUtc))
        {
            throw new AppException("Invalid token.", StatusCodes.Status400BadRequest);
        }

        return actionToken;
    }

    private async Task<AuthUser> GetRequiredUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.SingleOrDefaultAsync(x => x.Id == userId, cancellationToken);
        return user ?? throw new AppException("User not found.", StatusCodes.Status404NotFound);
    }

    private async Task<AuthUser?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var normalizedEmail = NormalizeEmail(email);
        return await dbContext.Users.SingleOrDefaultAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken);
    }

    private async Task EnsureEmailIsUniqueAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Users.AnyAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken);
        if (exists)
        {
            throw new AppException("Email is already used.", StatusCodes.Status409Conflict);
        }
    }

    private void EnsurePasswordIsValid(string password)
    {
        var result = passwordPolicyValidator.Validate(password);
        if (!result.IsValid)
        {
            throw new AppException(result.ErrorMessage ?? "Invalid password.", StatusCodes.Status400BadRequest);
        }
    }

    private static string NormalizeEmail(string email) => email.Trim().ToUpperInvariant();
    
    private static void EnsureStaffRole(UserRole role)
    {
        if (role is UserRole.Manager or UserRole.GeneralManager)
        {
            return;
        }

        throw new AppException("Staff role must be Manager or GeneralManager.", StatusCodes.Status400BadRequest);
    }

    private static void EnsureFacultyRule(UserRole role, Guid? facultyId)
    {
        if (role == UserRole.Manager && facultyId is null)
        {
            throw new AppException("FacultyId is required for Manager.", StatusCodes.Status400BadRequest);
        }

        if (role == UserRole.GeneralManager)
        {
            return;
        }
    }

    private async Task RevokeAllRefreshSessionsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var nowUtc = DateTimeOffset.UtcNow;
        var sessions = await dbContext.RefreshSessions
            .Where(x => x.UserId == userId && x.RevokedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var session in sessions)
        {
            session.RevokedAt = nowUtc;
        }
    }

    private string GenerateTemporaryPassword()
    {
        while (true)
        {
            var candidate = $"Tmp!{randomTokenGenerator.Generate(12)}aA1";
            candidate = candidate.Replace("-", "A", StringComparison.Ordinal);

            var result = passwordPolicyValidator.Validate(candidate);
            if (result.IsValid)
            {
                return candidate;
            }
        }
    }

    private async Task SaveAuthAndOutboxChangesAsync(Func<Task> publishAction, CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        await publishAction();
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}
