namespace Admission.Domain.Entities.Dictionary;

public class Faculty
{
    public Guid Id { get; set; }
    public DateTimeOffset CreateTime { get; set; }
    public string Name { get; set; } = string.Empty;
}