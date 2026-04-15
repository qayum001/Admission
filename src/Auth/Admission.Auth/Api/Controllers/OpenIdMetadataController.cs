using Admission.Auth.Options;
using Admission.Auth.Security.Signing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Admission.Auth.Api.Controllers;

/// <summary>
/// Exposes OpenID Connect discovery and JWKS metadata endpoints.
/// </summary>
[ApiController]
public sealed class OpenIdMetadataController(
    ISigningKeyCache signingKeyCache,
    IOptions<AuthOptions> authOptions) : ControllerBase
{
    private readonly AuthOptions _authOptions = authOptions.Value;

    /// <summary>
    /// Returns current JSON Web Key Set (JWKS) used by the auth service.
    /// </summary>
    [HttpGet("/.well-known/jwks.json")]
    [AllowAnonymous]
    public ActionResult<JwksDocument> GetJwks()
    {
        return Ok(new JwksDocument(signingKeyCache.GetJwksKeys()));
    }

    /// <summary>
    /// Returns OpenID Connect discovery metadata for this auth service.
    /// </summary>
    [HttpGet("/.well-known/openid-configuration")]
    [AllowAnonymous]
    public ActionResult<object> GetDiscovery()
    {
        var issuer = ResolveIssuer();

        return Ok(new
        {
            issuer,
            jwks_uri = $"{issuer}/.well-known/jwks.json",
            token_endpoint = $"{issuer}/api/auth/login",
            userinfo_endpoint = $"{issuer}/api/auth/me",
            grant_types_supported = new[] { "password", "refresh_token" },
            response_types_supported = new[] { "token" },
            subject_types_supported = new[] { "public" },
            id_token_signing_alg_values_supported = new[] { "RS256" },
            scopes_supported = new[] { "openid", "profile", "email", "roles" },
            claims_supported = new[] { "sub", "email", "role", "faculty_id" }
        });
    }

    private string ResolveIssuer()
    {
        if (Uri.TryCreate(_authOptions.Issuer, UriKind.Absolute, out var issuerUri))
        {
            return issuerUri.ToString().TrimEnd('/');
        }

        var scheme = HttpContext.Request.Scheme;
        var host = HttpContext.Request.Host.Value;
        return $"{scheme}://{host}";
    }
}
