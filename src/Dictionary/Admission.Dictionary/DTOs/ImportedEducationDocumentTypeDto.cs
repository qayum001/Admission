namespace Admission.Dictionary.DTOs;

public record ImportedEducationDocumentTypeDto(Guid Id, string Name, int EducationLevelId, List<int> NextEducationLevelsIds);
