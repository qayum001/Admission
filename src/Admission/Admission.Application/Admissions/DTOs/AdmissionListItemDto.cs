using Admission.Domain.Entities;

namespace Admission.Application.Admissions.DTOs;

public sealed class AdmissionListItemDto(Domain.Entities.Admission admission)
{
    public Guid Id { get; init; } = admission.Id;
    public AdmissionStatus Status { get; init; } = admission.AdmissionStatus;
    public DateTime LastUpdatedAt { get; init; } = admission.LastUpdatedAt;
    public Guid ApplicantId { get; init; } = admission.Applicant.Id;
    public string ApplicantName { get; init; } = admission.Applicant.Name.Value;
    public Guid? ManagerId { get; init; } = admission.Manager?.Id;
    public string? ManagerName { get; init; } = admission.Manager?.Name.Value;
    public int ProgramsCount { get; init; } = admission.AdmissionPrograms.Count;
}
