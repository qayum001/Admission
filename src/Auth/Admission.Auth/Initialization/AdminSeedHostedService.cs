using Admission.Auth.Domain.Entities;
using Admission.Auth.Options;
using Admission.Auth.Persistence;
using Admission.Auth.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Admission.Auth.Initialization;

public sealed class AdminSeedHostedService(
    IServiceScopeFactory scopeFactory,
    IOptions<AdminSeedOptions> adminSeedOptions,
    ILogger<AdminSeedHostedService> logger) : IHostedService
{
    private readonly AdminSeedOptions _options = adminSeedOptions.Value;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_options.Enabled)
        {
            logger.LogInformation("Admin seed is disabled");
            return;
        }

        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<AuthUser>>();
        var passwordPolicyValidator = scope.ServiceProvider.GetRequiredService<IPasswordPolicyValidator>();

        var normalizedEmail = _options.Email.Trim().ToUpperInvariant();
        var exists = await dbContext.Users.AnyAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken);
        if (exists)
        {
            logger.LogInformation("Admin seed skipped: account already exists");
            return;
        }

        var passwordValidation = passwordPolicyValidator.Validate(_options.Password);
        if (!passwordValidation.IsValid)
        {
            throw new InvalidOperationException($"Admin seed password is invalid: {passwordValidation.ErrorMessage}");
        }

        var admin = new AuthUser
        {
            Email = _options.Email.Trim(),
            NormalizedEmail = normalizedEmail,
            Role = _options.Role,
            EmailConfirmed = true
        };

        admin.PasswordHash = passwordHasher.HashPassword(admin, _options.Password);

        dbContext.Users.Add(admin);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Seeded admin account: {Email}", admin.Email);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
