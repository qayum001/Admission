namespace Admission.Auth.Security;

public sealed record PasswordPolicyValidationResult(bool IsValid, string? ErrorMessage)
{
    public static readonly PasswordPolicyValidationResult Valid = new(true, null);

    public static PasswordPolicyValidationResult Invalid(string message) => new(false, message);
}
