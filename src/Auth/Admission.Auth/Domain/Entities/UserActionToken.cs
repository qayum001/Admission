using Admission.Auth.Domain.Enums;

namespace Admission.Auth.Domain.Entities;

public sealed class UserActionToken
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid UserId { get; init; }
    public ActionTokenType Type { get; init; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? UsedAt { get; set; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    public AuthUser User { get; set; } = null!;

    public bool IsUsable(DateTimeOffset nowUtc) => UsedAt is null && ExpiresAt > nowUtc;
}
