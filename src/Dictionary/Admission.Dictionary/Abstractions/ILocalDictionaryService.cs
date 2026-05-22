using Admission.Domain.Entities.Dictionary;

namespace Admission.Dictionary.Abstractions;

public interface ILocalDictionaryService
{
    Task<List<EducationLevel>> GetEducationLevelsAsync();
    Task<List<EducationDocumentType>> GetDocumentTypesAsync();
    Task<List<Faculty>> GetFacultiesAsync();
    Task<List<EducationProgram>> GetProgramsAsync(
        Guid? facultyId = null,
        int? educationLevelId = null,
        string? language = null,
        string? educationForm = null,
        string? nameOrCode = null,
        int page = 1,
        int pageSize = 20);
    Task<List<EducationProgram>> GetProgramsByFacultyAsync(Guid facultyId);
}
