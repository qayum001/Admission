namespace Admission.Application.Applicants.DTOs;

public sealed record AddEducationalProgramDto(Guid ApplicantId, Guid ProgramId, int Priority, int MaxProgramsInAdmission);