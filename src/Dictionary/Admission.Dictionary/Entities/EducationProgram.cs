namespace Admission.Dictionary.Entities;

public class EducationProgram
{
    public Guid Id { get; set; }
    public DateTimeOffset CreateTime { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string EducationForm { get; set; } = string.Empty;
    public Faculty Faculty { get; set; } = new Faculty();
    public EducationLevel EducationLevel { get; set; } = new EducationLevel();
}