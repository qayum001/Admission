using Admission.Application.DTO;

namespace Admission.Application.Services;

public interface ILocalDictionaryService
{
    Task<IReadOnlyCollection<LocalDictionaryEducationLevelDto>> GetEducationLevelsAsync(CancellationToken ct = default);
    Task<IReadOnlyCollection<LocalDictionaryEducationDocumentTypeDto>> GetDocumentTypesAsync(CancellationToken ct = default);
    Task<IReadOnlyCollection<LocalDictionaryFacultyDto>> GetFacultiesAsync(CancellationToken ct = default);
    Task<IReadOnlyCollection<LocalDictionaryEducationProgramDto>> GetProgramsAsync(CancellationToken ct = default);
    Task<IReadOnlyCollection<LocalDictionaryEducationProgramDto>> GetProgramsByFacultyAsync(Guid facultyId, CancellationToken ct = default);
}
