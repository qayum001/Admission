using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Admission.Auth.Domain.Entities;
using Admission.Auth.Options;
using Admission.Auth.Security.Signing;
using Microsoft.Extensions.Options;

namespace Admission.Auth.Security;

public sealed class AccessTokenFactory(
    IOptions<AuthOptions> authOptions,
    ISigningKeyCache signingKeyCache) : IAccessTokenFactory
{
    private readonly AuthOptions _authOptions = authOptions.Value;
    private readonly JwtSecurityTokenHandler _tokenHandler = new();

    public IssuedAccessToken Create(AuthUser user)
    {
        var nowUtc = DateTimeOffset.UtcNow;
        var expiresAt = nowUtc.AddMinutes(_authOptions.AccessTokenMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(AuthClaimNames.Role, user.Role.ToString()),
            new(ClaimTypes.Role, user.Role.ToString()),
            new(AuthClaimNames.SecurityVersion, user.SecurityVersion.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
        };

        if (user.FacultyId.HasValue)
        {
            claims.Add(new Claim(AuthClaimNames.FacultyId, user.FacultyId.Value.ToString()));
        }

        var jwt = new JwtSecurityToken(
            issuer: _authOptions.Issuer,
            audience: _authOptions.Audience,
            claims: claims,
            notBefore: nowUtc.UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials: signingKeyCache.GetActiveSigningCredentials());

        var token = _tokenHandler.WriteToken(jwt);
        var expiresInSeconds = (int)Math.Round((expiresAt - nowUtc).TotalSeconds);

        return new IssuedAccessToken(token, expiresAt, expiresInSeconds);
    }
}
