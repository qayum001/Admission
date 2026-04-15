namespace Admission.Auth.Security;

public interface IPasswordPolicyValidator
{
    PasswordPolicyValidationResult Validate(string password);
}
