using Admission.Application.Managers.Commands;
using Admission.Application.Managers.Queries;
using Admission.Application.Services;
using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Admission.Api.Controllers;

/// <summary>
/// Manager profile and faculty assignment management.
/// </summary>
[ApiController]
[Route("api/managers")]
public sealed class ManagerController(
    ICommandMediator commandMediator,
    IQueryMediator queryMediator,
    ICurrentUserService currentUserService) : ControllerBase
{
    /// <summary>
    /// Creates the admission profile for the current manager or general manager.
    /// </summary>
    [HttpPost("profile")]
    [Authorize(Roles = "Manager,GeneralManager")]
    public async Task<ActionResult<Guid>> CreateProfile(
        [FromBody] CreateManagerProfileRequest request,
        CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated || !currentUserService.UserId.HasValue)
            return Unauthorized();

        var id = await commandMediator.SendAsync(
            new CreateManagerProfileCommand(
                currentUserService.UserId.Value,
                currentUserService.Role!,
                request.Name,
                currentUserService.Email!),
            cancellationToken);

        return CreatedAtAction(nameof(GetManagers), null, id);
    }

    /// <summary>
    /// Returns a paged list of manager profiles.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "GeneralManager,Admin")]
    public async Task<ActionResult<List<ManagerDto>>> GetManagers(
        [FromQuery] string? role = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var managers = await queryMediator.QueryAsync(
            new GetManagersQuery(role, page, pageSize), cancellationToken);

        return Ok(managers);
    }

    /// <summary>
    /// Assigns or clears the faculty for a manager.
    /// </summary>
    [HttpPatch("{managerId:guid}/faculty")]
    [Authorize(Roles = "GeneralManager,Admin")]
    public async Task<IActionResult> SetFaculty(
        Guid managerId,
        [FromBody] SetManagerFacultyRequest request,
        CancellationToken cancellationToken)
    {
        await commandMediator.SendAsync(
            new SetManagerFacultyCommand(managerId, request.FacultyId), cancellationToken);

        return NoContent();
    }
}

public sealed record CreateManagerProfileRequest(string Name);
public sealed record SetManagerFacultyRequest(Guid? FacultyId);
