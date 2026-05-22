using Admission.Domain.Entities;

namespace Admission.Application.Admissions.DTOs;

public sealed class ApplicantAdmissionDto(Domain.Entities.Admission admission)
{
    public Guid Id { get; init; } = admission.Id;
    public AdmissionStatus Status { get; init; } = admission.AdmissionStatus;
    public DateTime LastUpdatedAt { get; init; } = admission.LastUpdatedAt;
    public Guid ApplicantId { get; init; } = admission.Applicant.Id;
    public string ApplicantName { get; init; } = admission.Applicant.Name.Value;
    public Guid? ManagerId { get; init; } = admission.Manager?.Id;
    public string? ManagerName { get; init; } = admission.Manager?.Name.Value;
    public List<AdmissionProgramDto> Programs { get; init; } =
        admission.AdmissionPrograms
            .OrderBy(p => p.Priority)
            .Select(p => new AdmissionProgramDto(p))
            .ToList();
}
