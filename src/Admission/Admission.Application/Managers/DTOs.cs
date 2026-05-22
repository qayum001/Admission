using Admission.Domain.Entities;

namespace Admission.Application.Managers;

public sealed class AdmissionShortDto(Domain.Entities.Admission admission)
{
    public Guid Id { get; init; } = admission.Id;
    public Guid? ManagerId { get; init; } = admission.Manager?.Id;
    public bool HasPrograms { get; init; } = admission.AdmissionPrograms.Count != 0;
    public int ProgramsCount { get; init; } = admission.AdmissionPrograms.Count;
    public AdmissionStatus AdmissionStatus { get; init; } = admission.AdmissionStatus;
}