using Admission.Api.Contracts;
using Admission.Application.Applicants.Commands;
using Admission.Application.Applicants.DTOs;
using Admission.Application.Applicants.Queries;
using Admission.Application.DTO;
using Admission.Application.Services;
using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Admission.Api.Controllers;

/// <summary>
/// Educational document management.
/// </summary>
[ApiController]
[Route("api/applicants/{applicantId:guid}/educational-document")]
[Authorize(Roles = "Applicant,Manager,GeneralManager,Admin")]
public sealed class EducationalDocumentController(
    ICommandMediator commandMediator,
    IQueryMediator queryMediator,
    ICurrentUserService currentUserService,
    IFileValidationService fileValidationService) : ControllerBase
{
    /// <summary>
    /// Creates the educational document record and uploads the initial scan.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(
        Guid applicantId,
        [FromForm] CreateEducationalDocumentRequest request,
        CancellationToken cancellationToken)
    {
        var unauthorized = TryGetCurrentUserContext(out var userId, out var userRole);
        if (unauthorized is not null)
            return unauthorized;

        if (!await CanAccessApplicant(applicantId, userId, userRole, isEditOperation: true, cancellationToken))
            return Forbid();

        fileValidationService.Validate(request.File.FileName, request.File.ContentType, request.File.Length);

        await using var stream = request.File.OpenReadStream();
        var file = new FileDto(stream, request.File.FileName, request.File.ContentType);
        var documentId = await commandMediator.SendAsync(
            new CreateApplicantEducationalDocumentCommand(applicantId, request.EducationDocumentTypeId, file),
            cancellationToken);

        return CreatedAtAction(nameof(Get), new { applicantId }, documentId);
    }

    /// <summary>
    /// Updates the education document type.
    /// </summary>
    [HttpPatch("data")]
    public async Task<IActionResult> UpdateData(
        Guid applicantId,
        [FromBody] EditEducationalDocumentDataRequest request,
        CancellationToken cancellationToken)
    {
        var unauthorized = TryGetCurrentUserContext(out var userId, out var userRole);
        if (unauthorized is not null)
            return unauthorized;

        if (!await CanAccessApplicant(applicantId, userId, userRole, isEditOperation: true, cancellationToken))
            return Forbid();

        await commandMediator.SendAsync(
            new EditApplicantEducationalDocumentDataCommand(applicantId, request.EducationDocumentTypeId),
            cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Replaces the educational document scan. Returns the updated document with a fresh presigned download URL.
    /// </summary>
    [HttpPut("scan")]
    [RequestSizeLimit(20_000_000)]
    public async Task<ActionResult<EducationalDocumentDto>> UpdateScan(
        Guid applicantId,
        [FromForm] UpdateDocumentScanRequest request,
        CancellationToken cancellationToken)
    {
        var unauthorized = TryGetCurrentUserContext(out var userId, out var userRole);
        if (unauthorized is not null)
            return unauthorized;

        if (!await CanAccessApplicant(applicantId, userId, userRole, isEditOperation: true, cancellationToken))
            return Forbid();

        fileValidationService.Validate(request.File.FileName, request.File.ContentType, request.File.Length);

        await using var stream = request.File.OpenReadStream();
        var file = new FileDto(stream, request.File.FileName, request.File.ContentType);
        await commandMediator.SendAsync(
            new UpdateApplicantDocumentFileCommand(applicantId, file, DocumentType.EducationalDocument),
            cancellationToken);

        var document = await queryMediator.QueryAsync(
            new GetApplicantEducationalDocumentQuery(applicantId), cancellationToken);
        return Ok(document);
    }

    /// <summary>
    /// Deletes the educational document scan from storage.
    /// </summary>
    [HttpDelete("scan")]
    public async Task<IActionResult> DeleteScan(Guid applicantId, CancellationToken cancellationToken)
    {
        var unauthorized = TryGetCurrentUserContext(out var userId, out var userRole);
        if (unauthorized is not null)
            return unauthorized;

        if (!await CanAccessApplicant(applicantId, userId, userRole, isEditOperation: true, cancellationToken))
            return Forbid();

        await commandMediator.SendAsync(
            new DeleteApplicantDocumentFileCommand(applicantId, DocumentType.EducationalDocument),
            cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Returns educational document data with a presigned download URL for the scan.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<EducationalDocumentDto>> Get(Guid applicantId, CancellationToken cancellationToken)
    {
        var unauthorized = TryGetCurrentUserContext(out var userId, out var userRole);
        if (unauthorized is not null)
            return unauthorized;

        if (!await CanAccessApplicant(applicantId, userId, userRole, isEditOperation: false, cancellationToken))
            return Forbid();

        var document = await queryMediator.QueryAsync(new GetApplicantEducationalDocumentQuery(applicantId), cancellationToken);
        return Ok(document);
    }

    private ActionResult? TryGetCurrentUserContext(out Guid userId, out string userRole)
    {
        userId = Guid.Empty;
        userRole = string.Empty;

        if (!currentUserService.IsAuthenticated)
            return Unauthorized("User is not authenticated");

        if (!currentUserService.UserId.HasValue)
            return Unauthorized("User id claim is missing");

        if (string.IsNullOrWhiteSpace(currentUserService.Role))
            return Unauthorized("User role claim is missing");

        userId = currentUserService.UserId.Value;
        userRole = currentUserService.Role;
        return null;
    }

    private async Task<bool> CanAccessApplicant(
        Guid applicantId,
        Guid userId,
        string userRole,
        bool isEditOperation,
        CancellationToken cancellationToken)
    {
        return await queryMediator.QueryAsync(
            new CanUserAccessApplicantDataQuery(applicantId, userId, userRole, isEditOperation),
            cancellationToken);
    }
}
