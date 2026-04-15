using Admission.Auth.Domain.Enums;

namespace Admission.Auth.Domain.Entities;

public sealed class StaffInvitation
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string NormalizedEmail { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public Guid? FacultyId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? AcceptedAt { get; set; }
    public Guid CreatedByUserId { get; init; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    public bool IsUsable(DateTimeOffset nowUtc) => AcceptedAt is null && ExpiresAt > nowUtc;
}
