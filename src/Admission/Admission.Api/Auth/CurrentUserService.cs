using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Admission.Application.Services;

namespace Admission.Api.Auth;

internal sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid? UserId
    {
        get
        {
            var value = FindFirstValue(JwtRegisteredClaimNames.Sub)
                        ?? FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public string? Email =>
        FindFirstValue(JwtRegisteredClaimNames.Email)
        ?? FindFirstValue(ClaimTypes.Email);

    public string? Role =>
        FindFirstValue("role")
        ?? FindFirstValue(ClaimTypes.Role);

    public bool IsAuthenticated =>
        httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;

    public bool IsInRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
        {
            return false;
        }

        var principal = httpContextAccessor.HttpContext?.User;
        return principal?.IsInRole(role) == true;
    }

    private string? FindFirstValue(string claimType)
    {
        return httpContextAccessor.HttpContext?.User?.FindFirst(claimType)?.Value;
    }
}
