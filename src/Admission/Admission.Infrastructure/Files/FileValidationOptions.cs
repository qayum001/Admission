namespace Admission.Infrastructure.Files;

public sealed class FileValidationOptions
{
    public const string SectionName = "FileValidation";

    public long MaxSizeBytes { get; init; } = 10 * 1024 * 1024;

    public string[] AllowedContentTypes { get; init; } =
    [
        "application/pdf",
        "image/jpeg",
        "image/png"
    ];
}
