namespace Admission.Auth.Domain.Entities;

public sealed class RefreshSession
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid UserId { get; init; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public Guid? ReplacedBySessionId { get; set; }
    public int SecurityVersion { get; set; }
    public string? UserAgent { get; set; }
    public string? IpAddress { get; set; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    public AuthUser User { get; set; } = null!;

    public bool IsActive(DateTimeOffset nowUtc) => RevokedAt is null && ExpiresAt > nowUtc;
}
