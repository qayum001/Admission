using Dictionary.Integration;

namespace Admission.Dictionary.Abstractions;

public interface IDictionaryService
{
    public Task<ICollection<FacultyModel>> GetFacultiesAsync();
    public Task<ICollection<EducationLevelModel>> GetEducationLevelsAsync();
    public Task<ICollection<EducationDocumentTypeModel>> GetDocumentTypesAsync();
    public Task<ProgramPagedListModel> GetProgramsPageAsync(int page = 1, int size = 5);
}