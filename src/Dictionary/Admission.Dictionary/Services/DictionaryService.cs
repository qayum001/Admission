using Admission.Dictionary.Abstractions;
using Dictionary.Integration;

namespace Admission.Dictionary.Services;

public class DictionaryService(DictionaryClient client) : IDictionaryService
{
    public async Task<ICollection<FacultyModel>> GetFacultiesAsync()
    {
        var faculties = await client.FacultiesAsync();
        
        return faculties ?? [];
    }

    public async Task<ICollection<EducationLevelModel>> GetEducationLevelsAsync()
    {
        var levels = await client.Education_levelsAsync();
        
        return levels ?? [];
    }

    public async Task<ICollection<EducationDocumentTypeModel>> GetDocumentTypesAsync()
    {
        var res = await client.Document_typesAsync();
        
        return res ?? [];
    }

    public async Task<ProgramPagedListModel> GetProgramsPageAsync(int page = 1, int size = 5)
    {
        var res = await client.ProgramsAsync(page, size);

        return res;
    }
}