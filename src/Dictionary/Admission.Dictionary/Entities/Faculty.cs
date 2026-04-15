namespace Admission.Dictionary.Entities;

public class Faculty
{
    public Guid Id { get; set; }
    public DateTimeOffset CreateTime { get; set; }
    public string Name { get; set; } = string.Empty;
}