using Admission.Application.Exceptions;
using Admission.Application.Services;
using Admission.Domain.Entities.Dictionary;
using Microsoft.EntityFrameworkCore;

namespace Admission.Application.Common;

public sealed class DictionaryEntityResolver(
    IRepository repository,
    ILocalDictionaryService localDictionaryService)
{
    public async Task<EducationLevel> GetOrCreateLevelAsync(int id, string name, CancellationToken ct)
    {
        var local = await repository.EducationLevels
            .FirstOrDefaultAsync(l => l.Id == id, ct);

        if (local is not null)
            return local;

        var level = new EducationLevel { Id = id, Name = name };
        repository.EducationLevels.Add(level);
        return level;
    }

    public async Task<Faculty> GetOrCreateFacultyAsync(Guid id, CancellationToken ct)
    {
        var local = await repository.Faculties
            .FirstOrDefaultAsync(f => f.Id == id, ct);

        if (local is not null)
            return local;

        var faculties = await localDictionaryService.GetFacultiesAsync(ct);
        var dto = faculties.FirstOrDefault(f => f.Id == id)
            ?? throw new NotFoundException("Faculty not found in dictionary");

        var faculty = dto.ToFaculty();
        repository.Faculties.Add(faculty);
        return faculty;
    }

    public async Task<EducationDocumentType> GetOrCreateDocumentTypeAsync(Guid id, CancellationToken ct)
    {
        var local = await repository.EducationDocumentTypes
            .Include(t => t.EducationLevel)
            .Include(t => t.NextEducationLevels)
            .FirstOrDefaultAsync(t => t.Id == id, ct);

        if (local is not null)
            return local;

        var allTypes = await localDictionaryService.GetDocumentTypesAsync(ct);
        var dto = allTypes.FirstOrDefault(t => t.Id == id)
            ?? throw new NotFoundException("Education document type not found");

        var level = await GetOrCreateLevelAsync(dto.EducationLevel.Id, dto.EducationLevel.Name, ct);

        var nextLevels = new List<EducationLevel>();
        foreach (var nextDto in dto.NextEducationLevels)
        {
            var next = await GetOrCreateLevelAsync(nextDto.Id, nextDto.Name, ct);
            nextLevels.Add(next);
        }

        var docType = new EducationDocumentType
        {
            Id = dto.Id,
            Name = dto.Name,
            EducationLevel = level,
            NextEducationLevels = nextLevels
        };

        repository.EducationDocumentTypes.Add(docType);
        return docType;
    }

    public async Task<EducationProgram> GetOrCreateProgramAsync(Guid id, CancellationToken ct)
    {
        var local = await repository.Programs
            .Include(p => p.Faculty)
            .Include(p => p.EducationLevel)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (local is not null)
            return local;

        var allPrograms = await localDictionaryService.GetProgramsAsync(ct);
        var dto = allPrograms.FirstOrDefault(p => p.Id == id)
            ?? throw new NotFoundException("No such program found in imported programs list");

        var faculty = await GetOrCreateFacultyAsync(dto.Faculty.Id, ct);
        var level = await GetOrCreateLevelAsync(dto.EducationLevel.Id, dto.EducationLevel.Name, ct);

        var program = new EducationProgram
        {
            Id = dto.Id,
            Name = dto.Name,
            Code = dto.Code ?? string.Empty,
            Language = dto.Language ?? string.Empty,
            EducationForm = dto.EducationForm ?? string.Empty,
            Faculty = faculty,
            EducationLevel = level
        };

        repository.Programs.Add(program);
        return program;
    }
}
