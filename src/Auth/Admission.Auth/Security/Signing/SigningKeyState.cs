using Microsoft.IdentityModel.Tokens;

namespace Admission.Auth.Security.Signing;

public sealed record SigningKeyState(
    SigningCredentials ActiveSigningCredentials,
    IReadOnlyCollection<SecurityKey> ValidationKeys,
    IReadOnlyCollection<JwksKey> JwksKeys);
