using Admission.Application.DTO;
using Admission.Domain.Entities;

namespace Admission.Application.Applicants.DTOs;

public sealed class EducationalDocumentDto
{
    public Guid Id { get; init; }
    public Guid EducationDocumentTypeId { get; init; }
    public string EducationDocumentTypeName { get; init; } = string.Empty;
    public string? ScanUrl { get; init; }
    public DateTime? ScanUrlExpiresAt { get; init; }

    public static EducationalDocumentDto From(EducationalDocument document, PresignedUrlResult? scanUrl) => new()
    {
        Id = document.Id,
        EducationDocumentTypeId = document.EducationDocumentType.Id,
        EducationDocumentTypeName = document.EducationDocumentType.Name,
        ScanUrl = scanUrl?.Url,
        ScanUrlExpiresAt = scanUrl?.ExpiresAt
    };
}
