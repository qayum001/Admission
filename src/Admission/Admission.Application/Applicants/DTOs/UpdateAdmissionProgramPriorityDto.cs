namespace Admission.Application.Applicants.DTOs;

public sealed record UpdateAdmissionProgramPriorityDto(Guid ApplicantId, Guid AdmissionProgramId, int NewPriority, int MaxPriority);