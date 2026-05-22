namespace Admission.FileStorage.S3;

public sealed class S3FileStorageOptions
{
    public const string SectionName = "S3";

    public string ServiceUrl { get; init; } = string.Empty;
    
    public string? PublicServiceUrl { get; init; }

    public string AccessKey { get; init; } = string.Empty;
    public string SecretKey { get; init; } = string.Empty;
    public string BucketName { get; init; } = string.Empty;
    public bool ForcePathStyle { get; init; } = true;
    public bool UseHttp { get; init; }
}
