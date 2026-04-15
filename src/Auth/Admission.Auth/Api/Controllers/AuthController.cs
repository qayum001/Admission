using Admission.Auth.Api.Contracts;
using Admission.Auth.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Admission.Auth.Api.Controllers;

/// <summary>
/// Provides authentication endpoints for applicants and staff users.
/// </summary>
[ApiController]
[Route("api/auth")]
public sealed class AuthController(AuthService authService) : ControllerBase
{
    /// <summary>
    /// Registers a new applicant account.
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var response = await authService.RegisterApplicantAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    /// <summary>
    /// Authenticates a user and returns an access/refresh token pair.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await authService.LoginAsync(
            request,
            Request.Headers.UserAgent.ToString(),
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Exchanges an active refresh token for a new token pair.
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<RefreshResponse>> Refresh([FromBody] RefreshRequest request, CancellationToken cancellationToken)
    {
        var response = await authService.RefreshAsync(
            request.RefreshToken,
            Request.Headers.UserAgent.ToString(),
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Revokes the specified refresh token for the current user.
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<MessageResponse>> Logout([FromBody] LogoutRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetRequiredUserId();
        await authService.LogoutAsync(userId, request.RefreshToken, cancellationToken);
        return Ok(new MessageResponse("Logged out successfully"));
    }

    /// <summary>
    /// Revokes all active sessions for the current user.
    /// </summary>
    [HttpPost("logout-all")]
    [Authorize]
    public async Task<ActionResult<MessageResponse>> LogoutAll(CancellationToken cancellationToken)
    {
        var response = await authService.LogoutAllAsync(User.GetRequiredUserId(), cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Returns profile information for the current user.
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<MeResponse>> Me(CancellationToken cancellationToken)
    {
        var response = await authService.GetMeAsync(User.GetRequiredUserId(), cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Changes the current user's password.
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult<MessageResponse>> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        await authService.ChangePasswordAsync(User.GetRequiredUserId(), request, cancellationToken);
        return Ok(new MessageResponse("Password changed successfully"));
    }

    /// <summary>
    /// Updates the current user's email and triggers email confirmation for the new address.
    /// </summary>
    [HttpPatch("me/email")]
    [Authorize]
    public async Task<ActionResult<MessageResponse>> UpdateMyEmail([FromBody] UpdateMyEmailRequest request, CancellationToken cancellationToken)
    {
        await authService.UpdateMyEmailAsync(User.GetRequiredUserId(), request, cancellationToken);
        return Ok(new MessageResponse("Email updated successfully"));
    }

    /// <summary>
    /// Requests an email confirmation token for the specified account email.
    /// </summary>
    [HttpPost("email-confirmation/request")]
    [AllowAnonymous]
    public async Task<ActionResult<ActionTokenRequestedResponse>> RequestEmailConfirmation([FromBody] TokenRequest request, CancellationToken cancellationToken)
    {
        var response = await authService.RequestEmailConfirmationAsync(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Confirms email using a confirmation token payload.
    /// </summary>
    [HttpPost("email-confirmation/confirm")]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponse>> ConfirmEmail([FromBody] TokenConfirmRequest request, CancellationToken cancellationToken)
    {
        var response = await authService.ConfirmEmailAsync(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Confirms email using a token from confirmation link.
    /// </summary>
    [HttpGet("email-confirmation/{token}")]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponse>> ConfirmEmailByLink(string token, CancellationToken cancellationToken)
    {
        var response = await authService.ConfirmEmailAsync(token, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Requests a password reset token for the specified account email.
    /// </summary>
    [HttpPost("password-reset/request")]
    [AllowAnonymous]
    public async Task<ActionResult<ActionTokenRequestedResponse>> RequestPasswordReset([FromBody] TokenRequest request, CancellationToken cancellationToken)
    {
        var response = await authService.RequestPasswordResetAsync(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Resets password using a reset token and a new password.
    /// </summary>
    [HttpPost("password-reset/confirm")]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponse>> ConfirmPasswordReset([FromBody] TokenConfirmRequest request, CancellationToken cancellationToken)
    {
        var response = await authService.ConfirmPasswordResetAsync(request, cancellationToken);
        return Ok(response);
    }
}
