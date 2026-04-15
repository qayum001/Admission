using Admission.Auth.Options;
using Microsoft.Extensions.Options;

namespace Admission.Auth.Security;

public sealed class PasswordPolicyValidator(IOptions<PasswordPolicyOptions> options) : IPasswordPolicyValidator
{
    private readonly PasswordPolicyOptions _options = options.Value;

    public PasswordPolicyValidationResult Validate(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return PasswordPolicyValidationResult.Invalid("Password is required.");
        }

        if (password.Length < _options.MinLength)
        {
            return PasswordPolicyValidationResult.Invalid($"Password must be at least {_options.MinLength} characters long.");
        }

        if (_options.RequireUpper && !password.Any(char.IsUpper))
        {
            return PasswordPolicyValidationResult.Invalid("Password must contain an uppercase letter.");
        }

        if (_options.RequireLower && !password.Any(char.IsLower))
        {
            return PasswordPolicyValidationResult.Invalid("Password must contain a lowercase letter.");
        }

        if (_options.RequireDigit && !password.Any(char.IsDigit))
        {
            return PasswordPolicyValidationResult.Invalid("Password must contain a digit.");
        }

        if (_options.RequireNonAlphanumeric && password.All(char.IsLetterOrDigit))
        {
            return PasswordPolicyValidationResult.Invalid("Password must contain a non-alphanumeric character.");
        }

        if (_options.DisallowedPasswords.Any(x => string.Equals(x, password, StringComparison.OrdinalIgnoreCase)))
        {
            return PasswordPolicyValidationResult.Invalid("Password is too common.");
        }

        return PasswordPolicyValidationResult.Valid;
    }
}
