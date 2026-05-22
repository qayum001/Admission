using Admission.Application.DTO;
using Admission.Domain.Entities;

namespace Admission.Api.Contracts;

public sealed record CreateApplicantProfileRequest(
    string Name,
    DateTime BirthDate,
    Gender Gender,
    string PhoneNumber,
    string? Citizenship = null);

public sealed record EditApplicantProfileRequest(
    string? Name,
    DateTime? BirthDate,
    Gender? Gender,
    string? PhoneNumber,
    string? Email,
    string? Citizenship);

public sealed record EditApplicantPassportRequest(
    string? SerialNumber,
    DateTime? GivenDate,
    string? GivenBy);

public sealed record UpdateApplicantEducationLevelRequest(Guid EducationalDocumentId);

public sealed record RemoveAdmissionProgramRequest(Guid ProgramId, int MaxPrograms);

public sealed record UpdateApplicantDocumentFileRequest(IFormFile File, DocumentType DocumentType);

public sealed record CreatePassportRequest(
    string SerialNumber,
    DateTime GivenDate,
    string GivenBy,
    IFormFile File);

public sealed record UpdateDocumentScanRequest(IFormFile File);

public sealed record CreateEducationalDocumentRequest(
    Guid EducationDocumentTypeId,
    IFormFile File);

public sealed record EditEducationalDocumentDataRequest(Guid EducationDocumentTypeId);
