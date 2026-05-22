using Admission.Application.DTO;
using Admission.Application.Services;

namespace Admission.DictionaryClient.LocalDictionary;

internal sealed class LocalDictionaryService(HttpClient httpClient, DictionaryClientOptions options)
    : ILocalDictionaryService
{
    private readonly EducationClient _educationClient = new(options.BaseUrl, httpClient);
    private readonly DocumentClient _documentClient = new(options.BaseUrl, httpClient);
    private readonly Client _client = new(options.BaseUrl, httpClient);

    public async Task<IReadOnlyCollection<LocalDictionaryEducationLevelDto>> GetEducationLevelsAsync(CancellationToken ct = default)
    {
        var response = await _educationClient.LevelsGET2Async(ct);
        return response.Select(MapEducationLevel).ToArray();
    }

    public async Task<IReadOnlyCollection<LocalDictionaryEducationDocumentTypeDto>> GetDocumentTypesAsync(CancellationToken ct = default)
    {
        var response = await _documentClient.TypesGET2Async(ct);
        return response.Select(MapDocumentType).ToArray();
    }

    public async Task<IReadOnlyCollection<LocalDictionaryFacultyDto>> GetFacultiesAsync(CancellationToken ct = default)
    {
        var response = await _client.FacultiesGET2Async(ct);
        return response.Select(MapFaculty).ToArray();
    }

    public async Task<IReadOnlyCollection<LocalDictionaryEducationProgramDto>> GetProgramsAsync(CancellationToken ct = default)
    {
        var response = await _client.ProgramsAllGETAsync(ct);
        return response.Select(MapProgram).ToArray();
    }

    public async Task<IReadOnlyCollection<LocalDictionaryEducationProgramDto>> GetProgramsByFacultyAsync(Guid facultyId,
        CancellationToken ct = default)
    {
        var response = await _client.ProgramsAllGET2Async(facultyId, ct);
        return response.Select(MapProgram).ToArray();
    }

    private static LocalDictionaryEducationLevelDto MapEducationLevel(EducationLevel level)
    {
        return new LocalDictionaryEducationLevelDto
        {
            Id = level.Id,
            Name = level.Name ?? string.Empty
        };
    }

    private static LocalDictionaryFacultyDto MapFaculty(Faculty faculty)
    {
        return new LocalDictionaryFacultyDto
        {
            Id = faculty.Id,
            Name = faculty.Name ?? string.Empty
        };
    }

    private static LocalDictionaryEducationProgramDto MapProgram(EducationProgram program)
    {
        return new LocalDictionaryEducationProgramDto
        {
            Id = program.Id,
            Name = program.Name ?? string.Empty,
            Code = program.Code,
            Language = program.Language,
            EducationForm = program.EducationForm,
            Faculty = MapFaculty(program.Faculty),
            EducationLevel = MapEducationLevel(program.EducationLevel)
        };
    }

    private static LocalDictionaryEducationDocumentTypeDto MapDocumentType(EducationDocumentType type)
    {
        return new LocalDictionaryEducationDocumentTypeDto
        {
            Id = type.Id,
            Name = type.Name ?? string.Empty,
            EducationLevel = MapEducationLevel(type.EducationLevel),
            NextEducationLevels = type.NextEducationLevels?
                .Select(MapEducationLevel)
                .ToArray() ?? []
        };
    }
}
