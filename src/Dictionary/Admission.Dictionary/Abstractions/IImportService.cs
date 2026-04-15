using Admission.Dictionary.DTOs;

namespace Admission.Dictionary.Abstractions;

public interface IImportService
{
    Task<List<ImportedEducationLevelDto>> ImportEducationLevels(List<int> educationLevelsIds);
    Task<List<ImportedEducationDocumentTypeDto>> ImportDocumentTypes(List<Guid> documentTypesIds);
    Task<List<ImportedFacultyDto>> ImportFaculties(List<Guid> facultiesIds);
    Task<List<ImportedProgramDto>> ImportPrograms(List<ImportProgramsParams> importProgramsParams);
}
