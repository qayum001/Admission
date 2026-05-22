using Admission.Domain.Entities;

namespace Admission.Application.Admissions.DTOs;

public sealed class AdmissionProgramDto(AdmissionProgram admissionProgram)
{
    public Guid Id { get; init; } = admissionProgram.Id;
    public int Priority { get; init; } = admissionProgram.Priority;
    public Guid ProgramId { get; init; } = admissionProgram.ProgramId;
    public string ProgramName { get; init; } = admissionProgram.Program.Name;
    public string ProgramCode { get; init; } = admissionProgram.Program.Code;
    public string Language { get; init; } = admissionProgram.Program.Language;
    public string EducationForm { get; init; } = admissionProgram.Program.EducationForm;
    public Guid FacultyId { get; init; } = admissionProgram.Program.Faculty.Id;
    public string FacultyName { get; init; } = admissionProgram.Program.Faculty.Name;
}
