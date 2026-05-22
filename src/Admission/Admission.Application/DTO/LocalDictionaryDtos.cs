using Admission.Domain.Entities.Dictionary;

namespace Admission.Application.DTO;

public sealed record LocalDictionaryEducationLevelDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;

    public EducationLevel ToEducationLevel()
    {
        return new EducationLevel()
        {
            Id = Id,
            Name = Name
        };
    }
}

public sealed record LocalDictionaryFacultyDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;

    public Faculty ToFaculty()
    {
        return new Faculty()
        {
            Id = Id,
            Name = Name
        };
    }
}

public sealed record LocalDictionaryEducationProgramDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Code { get; init; }
    public string? Language { get; init; }
    public string? EducationForm { get; init; }
    public LocalDictionaryFacultyDto Faculty { get; init; } = new();
    public LocalDictionaryEducationLevelDto EducationLevel { get; init; } = new();

    public EducationProgram ToEducationProgram()
    {
        return new EducationProgram()
        {
            Id = Id,
            Name = Name,
            Code = Code,
            Language = Language,
            EducationForm = EducationForm,
            Faculty = Faculty.ToFaculty(),
            EducationLevel = EducationLevel.ToEducationLevel()
        };
    }
}

public sealed record LocalDictionaryEducationDocumentTypeDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public LocalDictionaryEducationLevelDto EducationLevel { get; init; } = new();
    public IReadOnlyCollection<LocalDictionaryEducationLevelDto> NextEducationLevels { get; init; }
        = Array.Empty<LocalDictionaryEducationLevelDto>();

    public EducationDocumentType ToEducationDocumentType()
    {
        return new EducationDocumentType()
        {
            Id = Id,
            Name = Name,
            EducationLevel = EducationLevel.ToEducationLevel(),
            NextEducationLevels = NextEducationLevels.Select(e => e.ToEducationLevel()).ToList()
        };
    }
}
