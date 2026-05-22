namespace Admission.Domain.Entities.Dictionary;

public class EducationDocumentType
{
    public Guid Id { get; set; }
    public DateTimeOffset CreateTime { get; set; }
    public string Name { get; set; } = string.Empty;
    public EducationLevel EducationLevel { get; set; } = new EducationLevel();
    public ICollection<EducationLevel> NextEducationLevels { get; set; } = [];
}