using Admission.Dictionary.Abstractions;
using Admission.Dictionary.DTOs;
using Admission.Dictionary.Entities;
using Admission.Dictionary.Persistence;
using Microsoft.EntityFrameworkCore;
using DictionaryIntegration = global::Dictionary.Integration;
using Npgsql;

namespace Admission.Dictionary.Services;

public class ImportService(DictionaryDbContext dictionaryContext, IDictionaryService externalDictionary) : IImportService
{
    public async Task<List<ImportedEducationLevelDto>> ImportEducationLevels(List<int> educationLevelsIds)
    {
        if (educationLevelsIds.Count < 1)
            return [];

        var requestedLevelIds = educationLevelsIds.ToHashSet();
        var existingLevelIds = await dictionaryContext.Levels
            .Where(e => requestedLevelIds.Contains(e.Id))
            .Select(e => e.Id)
            .ToListAsync();

        requestedLevelIds.ExceptWith(existingLevelIds);
        if (requestedLevelIds.Count < 1)
            return [];

        var loadedLevels = await externalDictionary.GetEducationLevelsAsync();
        var toImport = loadedLevels
            .Where(e => requestedLevelIds.Contains(e.Id))
            .GroupBy(e => e.Id)
            .Select(e => e.First())
            .Select(e => new EducationLevel
            {
                Id = e.Id,
                Name = e.Name
            })
            .ToList();

        if (toImport.Count < 1)
            return [];

        await dictionaryContext.Levels.AddRangeAsync(toImport);
        await dictionaryContext.SaveChangesAsync();

        return toImport
            .Select(e => new ImportedEducationLevelDto(e.Id, e.Name))
            .ToList();
    }

    public async Task<List<ImportedEducationDocumentTypeDto>> ImportDocumentTypes(List<Guid> documentTypesIds)
    {
        if (documentTypesIds.Count < 1)
            return [];

        var requestedDocumentTypeIds = documentTypesIds.ToHashSet();
        var existingDocumentTypeIds = await dictionaryContext.DocumentTypes
            .Where(e => requestedDocumentTypeIds.Contains(e.Id))
            .Select(e => e.Id)
            .ToListAsync();

        requestedDocumentTypeIds.ExceptWith(existingDocumentTypeIds);
        if (requestedDocumentTypeIds.Count < 1)
            return [];

        var loadedDocumentTypes = await externalDictionary.GetDocumentTypesAsync();
        var documentTypesToImport = loadedDocumentTypes
            .Where(e => requestedDocumentTypeIds.Contains(e.Id))
            .GroupBy(e => e.Id)
            .Select(e => e.First())
            .ToList();

        if (documentTypesToImport.Count < 1)
            return [];

        var levelModelsById = new Dictionary<int, DictionaryIntegration.EducationLevelModel>();
        foreach (var documentType in documentTypesToImport)
        {
            levelModelsById.TryAdd(documentType.EducationLevel.Id, documentType.EducationLevel);
            if (documentType.NextEducationLevels is null)
                continue;

            foreach (var nextLevel in documentType.NextEducationLevels)
                levelModelsById.TryAdd(nextLevel.Id, nextLevel);
        }

        var levelsById = await GetOrCreateEducationLevelsAsync(levelModelsById.Values);

        var toImport = documentTypesToImport
            .Where(e => levelsById.ContainsKey(e.EducationLevel.Id))
            .Select(e => new EducationDocumentType
            {
                Id = e.Id,
                Name = e.Name,
                CreateTime = e.CreateTime,
                EducationLevel = levelsById[e.EducationLevel.Id],
                NextEducationLevels = (e.NextEducationLevels ?? [])
                    .Where(l => levelsById.ContainsKey(l.Id))
                    .Select(l => levelsById[l.Id])
                    .DistinctBy(l => l.Id)
                    .ToList()
            })
            .ToList();

        await dictionaryContext.DocumentTypes.AddRangeAsync(toImport);
        await dictionaryContext.SaveChangesAsync();

        return toImport
            .Select(e => new ImportedEducationDocumentTypeDto(
                e.Id,
                e.Name,
                e.EducationLevel.Id,
                e.NextEducationLevels.Select(l => l.Id).ToList()))
            .ToList();
    }

    private async Task<Dictionary<int, EducationLevel>> GetOrCreateEducationLevelsAsync(IEnumerable<DictionaryIntegration.EducationLevelModel> levelModels)
    {
        var requiredLevels = levelModels
            .GroupBy(e => e.Id)
            .Select(e => e.First())
            .ToList();

        var requiredLevelIds = requiredLevels
            .Select(e => e.Id)
            .ToHashSet();
        var levelsById = await dictionaryContext.Levels
            .Where(e => requiredLevelIds.Contains(e.Id))
            .ToDictionaryAsync(e => e.Id);

        foreach (var levelModel in requiredLevels.Where(e => !levelsById.ContainsKey(e.Id)))
        {
            var level = new EducationLevel
            {
                Id = levelModel.Id,
                Name = levelModel.Name
            };

            await dictionaryContext.Levels.AddAsync(level);

            try
            {
                await dictionaryContext.SaveChangesAsync();
                levelsById[level.Id] = level;
            }
            catch (DbUpdateException exception) when (IsUniqueViolation(exception))
            {
                dictionaryContext.Entry(level).State = EntityState.Detached;

                var existingLevel = await dictionaryContext.Levels
                    .FirstOrDefaultAsync(e => e.Id == levelModel.Id);
                if (existingLevel is not null)
                    levelsById[existingLevel.Id] = existingLevel;
            }
        }

        return levelsById;
    }

    private static bool IsUniqueViolation(DbUpdateException exception)
    {
        return exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };
    }

    public async Task<List<ImportedFacultyDto>> ImportFaculties(List<Guid> facultiesIds)
    {
        if (facultiesIds.Count < 1)
            return [];

        var requestedFacultyIds = facultiesIds.ToHashSet();
        var existingFacultyIds = await dictionaryContext.Faculties
            .Where(e => requestedFacultyIds.Contains(e.Id))
            .Select(e => e.Id)
            .ToListAsync();

        requestedFacultyIds.ExceptWith(existingFacultyIds);
        if (requestedFacultyIds.Count < 1)
            return [];

        var loadedFaculties = await externalDictionary.GetFacultiesAsync();
        var toImport = loadedFaculties
            .Where(e => requestedFacultyIds.Contains(e.Id))
            .GroupBy(e => e.Id)
            .Select(e => e.First())
            .Select(e => new Faculty
            {
                Id = e.Id,
                Name = e.Name,
                CreateTime = e.CreateTime
            })
            .ToList();

        if (toImport.Count < 1)
            return [];

        await dictionaryContext.Faculties.AddRangeAsync(toImport);
        await dictionaryContext.SaveChangesAsync();

        return toImport
            .Select(e => new ImportedFacultyDto(e.Id, e.Name))
            .ToList();
    }

    public async Task<List<ImportedProgramDto>> ImportPrograms(List<ImportProgramsParams> programsParams)
    {
        if (programsParams.Count < 1)
            return [];

        var normalizedParams = programsParams
            .Where(e => e.Page > 0 && e.Size > 0 && e.Ids.Count > 0)
            .Select(e => new
            {
                e.Page,
                e.Size,
                Ids = e.Ids.ToHashSet()
            })
            .ToList();

        if (normalizedParams.Count < 1)
            return [];

        var requestedProgramIds = normalizedParams
            .SelectMany(e => e.Ids)
            .ToHashSet();
        var existingProgramIds = await dictionaryContext.Programs
            .Where(e => requestedProgramIds.Contains(e.Id))
            .Select(e => e.Id)
            .ToListAsync();

        requestedProgramIds.ExceptWith(existingProgramIds);
        if (requestedProgramIds.Count < 1)
            return [];

        var programModelsById = new Dictionary<Guid, DictionaryIntegration.EducationProgramModel>();
        foreach (var item in normalizedParams)
        {
            var programsPage = await externalDictionary.GetProgramsPageAsync(item.Page, item.Size);

            foreach (var program in programsPage.Programs ?? [])
            {
                if (!item.Ids.Contains(program.Id) || !requestedProgramIds.Contains(program.Id))
                    continue;

                programModelsById.TryAdd(program.Id, program);
            }
        }

        if (programModelsById.Count < 1)
            return [];

        var facultiesById = await GetOrCreateFacultiesAsync(programModelsById.Values.Select(e => e.Faculty));
        var levelsById = await GetOrCreateEducationLevelsAsync(programModelsById.Values.Select(e => e.EducationLevel));

        var toImport = programModelsById.Values
            .Where(e => facultiesById.ContainsKey(e.Faculty.Id) && levelsById.ContainsKey(e.EducationLevel.Id))
            .Select(e => new EducationProgram
            {
                Id = e.Id,
                Name = e.Name,
                CreateTime = e.CreateTime,
                Code = e.Code ?? string.Empty,
                Language = e.Language,
                EducationForm = e.EducationForm,
                Faculty = facultiesById[e.Faculty.Id],
                EducationLevel = levelsById[e.EducationLevel.Id]
            })
            .ToList();

        if (toImport.Count < 1)
            return [];

        await dictionaryContext.Programs.AddRangeAsync(toImport);
        await dictionaryContext.SaveChangesAsync();

        return toImport
            .Select(e => new ImportedProgramDto(e.Id, e.Name, e.Code))
            .ToList();
    }

    private async Task<Dictionary<Guid, Faculty>> GetOrCreateFacultiesAsync(IEnumerable<DictionaryIntegration.FacultyModel> facultyModels)
    {
        var requiredFaculties = facultyModels
            .GroupBy(e => e.Id)
            .Select(e => e.First())
            .ToList();

        var requiredFacultyIds = requiredFaculties
            .Select(e => e.Id)
            .ToHashSet();
        var facultiesById = await dictionaryContext.Faculties
            .Where(e => requiredFacultyIds.Contains(e.Id))
            .ToDictionaryAsync(e => e.Id);

        foreach (var facultyModel in requiredFaculties.Where(e => !facultiesById.ContainsKey(e.Id)))
        {
            var faculty = new Faculty
            {
                Id = facultyModel.Id,
                Name = facultyModel.Name,
                CreateTime = facultyModel.CreateTime
            };

            await dictionaryContext.Faculties.AddAsync(faculty);

            try
            {
                await dictionaryContext.SaveChangesAsync();
                facultiesById[faculty.Id] = faculty;
            }
            catch (DbUpdateException exception) when (IsUniqueViolation(exception))
            {
                dictionaryContext.Entry(faculty).State = EntityState.Detached;

                var existingFaculty = await dictionaryContext.Faculties
                    .FirstOrDefaultAsync(e => e.Id == facultyModel.Id);
                if (existingFaculty is not null)
                    facultiesById[existingFaculty.Id] = existingFaculty;
            }
        }

        return facultiesById;
    }
}
