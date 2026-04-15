using Microsoft.IdentityModel.Tokens;

namespace Admission.Auth.Security.Signing;

public interface ISigningKeyCache
{
    void Set(SigningKeyState state);
    SigningCredentials GetActiveSigningCredentials();
    IReadOnlyCollection<SecurityKey> GetValidationKeys();
    IReadOnlyCollection<JwksKey> GetJwksKeys();
}
