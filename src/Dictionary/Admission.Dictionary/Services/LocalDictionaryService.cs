using Admission.Dictionary.Abstractions;
using Admission.Domain.Entities.Dictionary;
using Admission.Dictionary.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Admission.Dictionary.Services;

public class LocalDictionaryService(DictionaryDbContext context) : ILocalDictionaryService
{
    public async Task<List<EducationLevel>> GetEducationLevelsAsync()
    {
        return await context.Levels
            .OrderBy(e => e.Id)
            .ToListAsync();
    }

    public async Task<List<EducationDocumentType>> GetDocumentTypesAsync()
    {
        return await context.DocumentTypes
            .Include(e => e.EducationLevel)
            .Include(e => e.NextEducationLevels)
            .OrderBy(e => e.Name)
            .ToListAsync();
    }

    public async Task<List<Faculty>> GetFacultiesAsync()
    {
        return await context.Faculties
            .OrderBy(e => e.Name)
            .ToListAsync();
    }

    public async Task<List<EducationProgram>> GetProgramsAsync(
        Guid? facultyId = null,
        int? educationLevelId = null,
        string? language = null,
        string? educationForm = null,
        string? nameOrCode = null,
        int page = 1,
        int pageSize = 20)
    {
        var query = context.Programs
            .Include(e => e.Faculty)
            .Include(e => e.EducationLevel)
            .AsQueryable();

        if (facultyId.HasValue)
            query = query.Where(e => e.Faculty.Id == facultyId.Value);

        if (educationLevelId.HasValue)
            query = query.Where(e => e.EducationLevel.Id == educationLevelId.Value);

        if (!string.IsNullOrWhiteSpace(language))
            query = query.Where(e => e.Language != null && e.Language.ToLower().Contains(language.ToLower()));

        if (!string.IsNullOrWhiteSpace(educationForm))
            query = query.Where(e => e.EducationForm != null && e.EducationForm.ToLower().Contains(educationForm.ToLower()));

        if (!string.IsNullOrWhiteSpace(nameOrCode))
            query = query.Where(e =>
                (e.Name != null && e.Name.ToLower().Contains(nameOrCode.ToLower())) ||
                (e.Code != null && e.Code.ToLower().Contains(nameOrCode.ToLower())));

        return await query
            .OrderBy(e => e.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<EducationProgram>> GetProgramsByFacultyAsync(Guid facultyId)
    {
        return await context.Programs
            .Include(e => e.Faculty)
            .Include(e => e.EducationLevel)
            .Where(e => e.Faculty.Id == facultyId)
            .OrderBy(e => e.Name)
            .ToListAsync();
    }
}
