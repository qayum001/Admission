using Admission.Api.Contracts;
using Admission.Api.Options;
using Admission.Application.Admissions.Commands;
using Admission.Application.Admissions.DTOs;
using Admission.Application.Admissions.Queries;
using Admission.Application.Applicants.Commands;
using Admission.Application.Applicants.DTOs;
using Admission.Application.Applicants.Queries;
using Admission.Application.Managers.Commands;
using Admission.Application.Managers.Queries;
using Admission.Application.Services;
using Admission.Domain.Entities;
using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Admission.Api.Controllers;

/// <summary>
/// Admission and program management.
/// </summary>
[ApiController]
[Route("api/admissions")]
public class AdmissionController(
    ICommandMediator commandMediator,
    IQueryMediator queryMediator,
    ICurrentUserService currentUserService,
    AdmissionProgramsOptions programsOptions) : ControllerBase
{
    /// <summary>
    /// Returns the current applicant's admission with selected programs.
    /// </summary>
    [HttpGet("me")]
    [Authorize(Roles = "Applicant")]
    public async Task<ActionResult<ApplicantAdmissionDto>> GetMyAdmission(CancellationToken cancellationToken)
    {
        var userError = TryGetRequiredUserContext(out var externalId, out _, out _);
        if (userError is not null) return userError;

        var profile = await queryMediator.QueryAsync(
            new GetCurrentApplicantProfileQuery(externalId), cancellationToken);

        var admission = await queryMediator.QueryAsync(
            new GetApplicantAdmissionQuery(profile.Id), cancellationToken);

        return Ok(admission);
    }

    /// <summary>
    /// Adds an education program to the current applicant's admission.
    /// </summary>
    [HttpPost("programs")]
    [Authorize(Roles = "Applicant")]
    public async Task<ActionResult<AdmissionProgramDto>> AddProgram(
        [FromBody] AddAdmissionProgramRequest request,
        CancellationToken cancellationToken)
    {
        var userError = TryGetRequiredUserContext(out var externalId, out _, out _);
        if (userError is not null) return userError;

        var profile = await queryMediator.QueryAsync(
            new GetCurrentApplicantProfileQuery(externalId), cancellationToken);

        var canEdit = await queryMediator.QueryAsync(
            new CanUserAccessApplicantDataQuery(profile.Id, externalId, "Applicant", IsEditOperation: true),
            cancellationToken);

        if (!canEdit) return Forbid();

        var dto = new AddEducationalProgramDto(
            profile.Id,
            request.ProgramId,
            request.Priority,
            programsOptions.MaxSelectedPrograms);

        var admissionProgram = await commandMediator.SendAsync(
            new AddEducationProgramToAdmissionCommand(dto), cancellationToken);

        return CreatedAtAction(nameof(GetMyAdmission), new AdmissionProgramDto(admissionProgram));
    }

    /// <summary>
    /// Changes the priority of a program in the current applicant's admission.
    /// </summary>
    [HttpPatch("programs/{admissionProgramId:guid}/priority")]
    [Authorize(Roles = "Applicant")]
    public async Task<IActionResult> UpdateProgramPriority(
        Guid admissionProgramId,
        [FromBody] UpdatePriorityRequest request,
        CancellationToken cancellationToken)
    {
        var userError = TryGetRequiredUserContext(out var externalId, out _, out _);
        if (userError is not null) return userError;

        var profile = await queryMediator.QueryAsync(
            new GetCurrentApplicantProfileQuery(externalId), cancellationToken);

        var canEdit = await queryMediator.QueryAsync(
            new CanUserAccessApplicantDataQuery(profile.Id, externalId, "Applicant", IsEditOperation: true),
            cancellationToken);

        if (!canEdit) return Forbid();

        var dto = new UpdateAdmissionProgramPriorityDto(
            profile.Id,
            admissionProgramId,
            request.Priority,
            programsOptions.MaxSelectedPrograms);

        await commandMediator.SendAsync(
            new UpdateAdmissionProgramPriorityCommand(dto), cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Removes a program from the current applicant's admission.
    /// </summary>
    [HttpDelete("programs/{admissionProgramId:guid}")]
    [Authorize(Roles = "Applicant")]
    public async Task<IActionResult> RemoveProgram(
        Guid admissionProgramId,
        CancellationToken cancellationToken)
    {
        var userError = TryGetRequiredUserContext(out var externalId, out _, out _);
        if (userError is not null) return userError;

        var profile = await queryMediator.QueryAsync(
            new GetCurrentApplicantProfileQuery(externalId), cancellationToken);

        var canEdit = await queryMediator.QueryAsync(
            new CanUserAccessApplicantDataQuery(profile.Id, externalId, "Applicant", IsEditOperation: true),
            cancellationToken);

        if (!canEdit) return Forbid();

        await commandMediator.SendAsync(
            new RemoveAdmissionProgramCommand(profile.Id, admissionProgramId, programsOptions.MaxSelectedPrograms),
            cancellationToken);

        return NoContent();
    }
    
    /// <summary>
    /// Returns a paginated, filtered list of all admissions.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Manager,GeneralManager,Admin")]
    public async Task<ActionResult<PagedResult<AdmissionListItemDto>>> GetAllAdmissions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? applicantName = null,
        [FromQuery] Guid? programId = null,
        [FromQuery] Guid? facultyId = null,
        [FromQuery] AdmissionStatus? status = null,
        [FromQuery] bool? withoutManager = null,
        [FromQuery] DateTime? lastUpdatedAfter = null,
        [FromQuery] bool myAdmissionsOnly = false,
        [FromQuery] bool sortAscending = false,
        CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        Guid? filterManagerId = null;
        if (myAdmissionsOnly)
        {
            var userError = TryGetRequiredUserContext(out var extId, out _, out _);
            if (userError is not null) return userError;
            try
            {
                filterManagerId = await queryMediator.QueryAsync(
                    new GetManagerByExternalIdQuery(extId), cancellationToken);
            }
            catch
            {
                return Ok(new PagedResult<AdmissionListItemDto>
                    { Items = [], Page = page, PageSize = pageSize, TotalCount = 0 });
            }
        }

        var result = await queryMediator.QueryAsync(
            new GetAllAdmissionsQuery(
                page, pageSize, applicantName, programId, facultyId,
                status, withoutManager, lastUpdatedAfter,
                filterManagerId, sortAscending),
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Returns a specific admission by ID.
    /// </summary>
    [HttpGet("{admissionId:guid}")]
    [Authorize(Roles = "Manager,GeneralManager,Admin")]
    public async Task<ActionResult<ApplicantAdmissionDto>> GetAdmissionById(
        Guid admissionId,
        CancellationToken cancellationToken)
    {
        var admission = await queryMediator.QueryAsync(
            new GetAdmissionByIdQuery(admissionId), cancellationToken);

        return Ok(admission);
    }

    /// <summary>
    /// Assigns a manager to an admission.
    /// </summary>
    [HttpPost("{admissionId:guid}/manager")]
    [Authorize(Roles = "Manager,GeneralManager")]
    public async Task<IActionResult> AttachManager(
        Guid admissionId,
        [FromBody] AttachManagerRequest? request,
        CancellationToken cancellationToken)
    {
        var userError = TryGetRequiredUserContext(out var externalId, out var role, out _);
        if (userError is not null) return userError;

        Guid targetManagerId;

        if (string.Equals(role, "GeneralManager", StringComparison.OrdinalIgnoreCase)
            && request?.ManagerId.HasValue == true)
        {
            targetManagerId = request.ManagerId.Value;
        }
        else
        {
            targetManagerId = await queryMediator.QueryAsync(
                new GetManagerByExternalIdQuery(externalId), cancellationToken);
        }

        await commandMediator.SendAsync(
            new AttachAdmissionToManagerCommand(targetManagerId, admissionId), cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Removes the manager assignment from an admission.
    /// </summary>
    [HttpDelete("{admissionId:guid}/manager")]
    [Authorize(Roles = "Manager,GeneralManager")]
    public async Task<IActionResult> DetachManager(
        Guid admissionId,
        [FromBody] DetachManagerRequest? request,
        CancellationToken cancellationToken)
    {
        var userError = TryGetRequiredUserContext(out var externalId, out var role, out _);
        if (userError is not null) return userError;

        Guid targetManagerId;

        if (string.Equals(role, "GeneralManager", StringComparison.OrdinalIgnoreCase)
            && request?.ManagerId.HasValue == true)
        {
            targetManagerId = request.ManagerId.Value;
        }
        else
        {
            targetManagerId = await queryMediator.QueryAsync(
                new GetManagerByExternalIdQuery(externalId), cancellationToken);
        }

        await commandMediator.SendAsync(
            new DetachAdmissionFromManagerCommand(targetManagerId, admissionId), cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Updates the admission status.
    /// </summary>
    [HttpPatch("{admissionId:guid}/status")]
    [Authorize(Roles = "Manager,GeneralManager,Admin")]
    public async Task<IActionResult> UpdateStatus(
        Guid admissionId,
        [FromBody] UpdateAdmissionStatusRequest request,
        CancellationToken cancellationToken)
    {
        var userError = TryGetRequiredUserContext(out _, out _, out _);
        if (userError is not null) return userError;

        await commandMediator.SendAsync(
            new UpdateAdmissionStatusCommand(admissionId, request.Status), cancellationToken);

        return NoContent();
    }
    
    /// <summary>
    /// Changes the priority of a program in a specific admission.
    /// </summary>
    [HttpPatch("{admissionId:guid}/programs/{admissionProgramId:guid}/priority")]
    [Authorize(Roles = "Manager,GeneralManager,Admin")]
    public async Task<IActionResult> ManagerUpdateProgramPriority(
        Guid admissionId,
        Guid admissionProgramId,
        [FromBody] UpdatePriorityRequest request,
        CancellationToken cancellationToken)
    {
        var userError = TryGetRequiredUserContext(out var externalId, out var role, out _);
        if (userError is not null) return userError;

        var applicantId = await queryMediator.QueryAsync(
            new GetApplicantIdByAdmissionProgramIdQuery(admissionProgramId), cancellationToken);

        var canEdit = await queryMediator.QueryAsync(
            new CanUserAccessApplicantDataQuery(applicantId, externalId, role, IsEditOperation: true),
            cancellationToken);

        if (!canEdit) return Forbid();

        var dto = new UpdateAdmissionProgramPriorityDto(
            applicantId, admissionProgramId, request.Priority, programsOptions.MaxSelectedPrograms);

        await commandMediator.SendAsync(new UpdateAdmissionProgramPriorityCommand(dto), cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Removes a program from a specific admission.
    /// </summary>
    [HttpDelete("{admissionId:guid}/programs/{admissionProgramId:guid}")]
    [Authorize(Roles = "Manager,GeneralManager,Admin")]
    public async Task<IActionResult> ManagerRemoveProgram(
        Guid admissionId,
        Guid admissionProgramId,
        CancellationToken cancellationToken)
    {
        var userError = TryGetRequiredUserContext(out var externalId, out var role, out _);
        if (userError is not null) return userError;

        var applicantId = await queryMediator.QueryAsync(
            new GetApplicantIdByAdmissionProgramIdQuery(admissionProgramId), cancellationToken);

        var canEdit = await queryMediator.QueryAsync(
            new CanUserAccessApplicantDataQuery(applicantId, externalId, role, IsEditOperation: true),
            cancellationToken);

        if (!canEdit) return Forbid();

        await commandMediator.SendAsync(
            new RemoveAdmissionProgramCommand(applicantId, admissionProgramId, programsOptions.MaxSelectedPrograms),
            cancellationToken);

        return NoContent();
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
