namespace Admission.Auth.Api.Contracts;

public sealed record RegisterRequest(
    string Email,
    string Password);

public sealed record RegisterResponse(Guid UserId, string Role, string Message);

public sealed record LoginRequest(
    string Email,
    string Password);

public sealed record LoginResponse(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    string Role,
    bool MustChangePassword);

public sealed record RefreshRequest(string RefreshToken);

public sealed record RefreshResponse(string AccessToken, string RefreshToken, int ExpiresIn);

public sealed record LogoutRequest(string RefreshToken);

public sealed record MessageResponse(string Message);

public sealed record MeResponse(Guid UserId, string Email, string Role, Guid? FacultyId, bool EmailConfirmed, bool MustChangePassword);

public sealed record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword);

public sealed record UpdateMyEmailRequest(string Email);

public sealed record StaffCreateRequest(
    string Email,
    string Role,
    Guid? FacultyId);

public sealed record StaffCreateResponse(Guid UserId, string Role, string TemporaryPassword, string Message);

public sealed record StaffUpdateRequest(
    string Email,
    string Role,
    Guid? FacultyId);

public sealed record StaffListItem(Guid Id, string Email, string Role, Guid? FacultyId);

public sealed record StaffListResponse(IReadOnlyCollection<StaffListItem> Items, PaginationResponse Pagination);

public sealed record PaginationResponse(int Page, int Size, int Count);

public sealed record ForceResetPasswordResponse(string TemporaryPassword, string Message);

public sealed record InvitationCreateRequest(
    string Email,
    string Role,
    Guid? FacultyId);

public sealed record InvitationCreateResponse(string Message, string? DebugInvitationToken = null);

public sealed record AcceptInvitationRequest(
    string InvitationToken,
    string Password);

public sealed record TokenRequest(string Email);

public sealed record TokenConfirmRequest(
    string Token,
    string? NewPassword);

public sealed record ActionTokenRequestedResponse(string Message, string? DebugToken = null);
