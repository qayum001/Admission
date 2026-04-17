using Admission.Auth.Api.Contracts;
using Admission.Auth.Application;
using Admission.Auth.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Admission.Auth.Api.Controllers;

/// <summary>
/// Provides staff user management endpoints.
/// </summary>
[ApiController]
[Route("api/auth/staff-users")]
[Authorize(Roles = "Admin")]
public sealed class StaffUsersController(AuthService authService) : ControllerBase
{
    /// <summary>
    /// Creates a new staff user account.
    /// </summary>
    /// <param name="request">Available roles: GeneralManager, Manager</param>
    [HttpPost]
    public async Task<ActionResult<StaffCreateResponse>> Create([FromBody] StaffCreateRequest request, CancellationToken cancellationToken)
    {
        var response = await authService.CreateStaffAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, response);
    }
    
    /// <summary>
    /// Deletes a staff user.
    /// </summary>
    [HttpDelete("{userId:guid}")]
    public async Task<ActionResult<MessageResponse>> Delete(Guid userId, CancellationToken cancellationToken)
    {
        var response = await authService.DeleteStaffAsync(userId, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Returns a paged list of staff users.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "GeneralManager,Admin")]
    public async Task<ActionResult<StaffListResponse>> List([FromQuery] UserRole? role, [FromQuery] int page = 1, [FromQuery] int size = 20, CancellationToken cancellationToken = default)
    {
        var response = await authService.GetStaffListAsync(role, page, size, cancellationToken);
        return Ok(response);
    }
}
