namespace Admission.Dictionary.DTOs;

public record ImportProgramsParams(List<Guid> Ids, int Page, int Size);