using System.Security.Claims;
using Admission.Auth.Common;
using System.IdentityModel.Tokens.Jwt;

namespace Admission.Auth.Api;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetRequiredUserId(this ClaimsPrincipal principal)
    {
        var sub = principal.FindFirstValue(JwtRegisteredClaimNames.Sub)
                  ?? principal.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(sub, out var userId))
        {
            throw new AppException("Invalid user claim.", StatusCodes.Status401Unauthorized);
        }

        return userId;
    }
}
