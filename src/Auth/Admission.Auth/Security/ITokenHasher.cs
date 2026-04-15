namespace Admission.Auth.Security;

public interface ITokenHasher
{
    string Hash(string token);
}
