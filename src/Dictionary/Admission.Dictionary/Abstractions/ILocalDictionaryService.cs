using Admission.Dictionary.Entities;

namespace Admission.Dictionary.Abstractions;

public interface ILocalDictionaryService
{
    Task<List<EducationLevel>> GetEducationLevelsAsync();
    Task<List<EducationDocumentType>> GetDocumentTypesAsync();
    Task<List<Faculty>> GetFacultiesAsync();
    Task<List<EducationProgram>> GetProgramsAsync();
    Task<List<EducationProgram>> GetProgramsByFacultyAsync(Guid facultyId);
}
