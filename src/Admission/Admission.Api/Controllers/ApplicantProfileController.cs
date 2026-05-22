using Admission.Api.Contracts;
using Admission.Application.Applicants.Commands;
using Admission.Application.Applicants.DTOs;
using Admission.Application.Applicants.Queries;
using Admission.Application.Services;
using Admission.Domain.Entities;
using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Admission.Api.Controllers;

/// <summary>
/// Applicant profile management.
/// </summary>
[ApiController]
[Route("api/profiles")]
public class ApplicantProfileController(
    ICommandMediator commandMediator,
    IQueryMediator queryMediator,
    ICurrentUserService currentUserService) : ControllerBase
{
    /// <summary>
    /// Creates the applicant's profile.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Applicant")]
    public async Task<ActionResult<Guid>> CreateProfile(
        [FromBody] CreateApplicantProfileRequest request,
        CancellationToken cancellationToken)
    {
        var userContextError = TryGetRequiredUserContext(out var externalId, out var role, out var email);
        if (userContextError is not null)
            return userContextError;

        var dto = new CreateApplicantProfileDto(
            request.Name,
            request.BirthDate,
            request.Gender,
            request.PhoneNumber,
            request.Citizenship);

        var id = await commandMediator.SendAsync(
            new CreateApplicantProfileCommand(externalId, role, email, dto),
            cancellationToken);

        return CreatedAtAction(nameof(GetMe), null, id);
    }

    /// <summary>
    /// Updates the current applicant's own profile.
    /// </summary>
    [HttpPatch]
    [Authorize(Roles = "Applicant")]
    public async Task<IActionResult> EditMyProfile(
        [FromBody] EditApplicantProfileRequest request,
        CancellationToken cancellationToken)
    {
        var userContextError = TryGetRequiredUserContext(out var externalId, out _, out _);
        if (userContextError is not null)
            return userContextError;

        var profile = await queryMediator.QueryAsync(
            new GetCurrentApplicantProfileQuery(externalId),
            cancellationToken);

        var canEdit = await queryMediator.QueryAsync(
            new CanUserAccessApplicantDataQuery(profile.Id, externalId, "Applicant", IsEditOperation: true),
            cancellationToken);

        if (!canEdit)
            return Forbid();

        var payload = new EditApplicantData(
            request.Name,
            request.BirthDate,
            request.Gender,
            request.PhoneNumber,
            request.Email,
            request.Citizenship);

        await commandMediator.SendAsync(
            new EditCurrentApplicantProfileCommand(externalId, payload),
            cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Updates a specific applicant's profile.
    /// </summary>
    [HttpPatch("{applicantId:guid}")]
    [Authorize(Roles = "Manager,GeneralManager,Admin")]
    public async Task<IActionResult> EditApplicantProfileByManagerOrAdmin(
        Guid applicantId,
        [FromBody] EditApplicantProfileRequest request,
        CancellationToken cancellationToken)
    {
        var userContextError = TryGetRequiredUserContext(out var externalId, out var role, out _);
        if (userContextError is not null)
            return userContextError;

        var canEdit = await queryMediator.QueryAsync(
            new CanUserAccessApplicantDataQuery(applicantId, externalId, role, IsEditOperation: true),
            cancellationToken);

        if (!canEdit)
            return Forbid();

        var payload = new EditApplicantData(
            request.Name,
            request.BirthDate,
            request.Gender,
            request.PhoneNumber,
            request.Email,
            request.Citizenship);

        await commandMediator.SendAsync(
            new EditApplicantProfileCommand(applicantId, payload),
            cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Returns the profile of a specific applicant.
    /// </summary>
    [HttpGet("{applicantId:guid}")]
    [Authorize(Roles = "Manager,GeneralManager,Admin")]
    public async Task<ActionResult<ApplicantDto>> GetApplicantProfile(
        Guid applicantId,
        CancellationToken cancellationToken)
    {
        var userContextError = TryGetRequiredUserContext(out var externalId, out var role, out _);
        if (userContextError is not null)
            return userContextError;

        var canAccess = await queryMediator.QueryAsync(
            new CanUserAccessApplicantDataQuery(applicantId, externalId, role, IsEditOperation: false),
            cancellationToken);

        if (!canAccess)
            return Forbid();

        var profile = await queryMediator.QueryAsync(
            new GetApplicantByIdQuery(applicantId),
            cancellationToken);

        return Ok(profile);
    }

    /// <summary>
    /// Returns the current applicant's own profile.
    /// </summary>
    [HttpGet("me")]
    [Authorize(Roles = "Applicant")]
    public async Task<ActionResult<ApplicantDto>> GetMe(CancellationToken cancellationToken)
    {
        var userContextError = TryGetRequiredUserContext(out var externalId, out _, out _);
        if (userContextError is not null)
            return userContextError;

        var profile = await queryMediator.QueryAsync(
            new GetCurrentApplicantProfileQuery(externalId),
            cancellationToken);

        return Ok(profile);
    }

    private ActionResult? TryGetRequiredUserContext(out Guid externalId, out string role, out string email)
    {
        externalId = Guid.Empty;
        role = string.Empty;
        email = string.Empty;

        if (!currentUserService.IsAuthenticated)
            return Unauthorized("User is not authenticated");

        if (!currentUserService.UserId.HasValue)
            return Unauthorized("User id claim is missing");

        if (string.IsNullOrWhiteSpace(currentUserService.Role))
            return Unauthorized("User role claim is missing");

        if (string.IsNullOrWhiteSpace(currentUserService.Email))
            return Unauthorized("User email claim is missing");

        externalId = currentUserService.UserId.Value;
        role = currentUserService.Role;
        email = currentUserService.Email;

        return null;
    }
}
