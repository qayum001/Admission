using Admission.Dictionary.Abstractions;
using Admission.Dictionary.Entities;
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

    public async Task<List<EducationProgram>> GetProgramsAsync()
    {
        return await context.Programs
            .Include(e => e.Faculty)
            .Include(e => e.EducationLevel)
            .OrderBy(e => e.Name)
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
