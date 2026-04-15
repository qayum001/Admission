namespace Admission.Auth.Domain.Entities;

public sealed class SigningKeyRecord
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Kid { get; set; } = string.Empty;
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string PrivateKeyPem { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ActivatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? RetiredAt { get; set; }
    public bool IsActive { get; set; }
}
