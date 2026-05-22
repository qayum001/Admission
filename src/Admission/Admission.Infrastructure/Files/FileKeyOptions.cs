namespace Admission.Infrastructure.Files;

public sealed class FileKeyOptions
{
    public const string SectionName = "FileStorage";

    public string RootPrefix { get; init; } = "admission";
}
