using Admission.Domain.Entities;

namespace Admission.Api.Contracts;

public sealed record AddAdmissionProgramRequest(Guid ProgramId, int Priority);

public sealed record UpdatePriorityRequest(int Priority);

public sealed record AttachManagerRequest(Guid? ManagerId);

public sealed record DetachManagerRequest(Guid? ManagerId);

public sealed record UpdateAdmissionStatusRequest(AdmissionStatus Status);
