namespace Admission.Application.DTO;

public record PresignedUrlResult
{
    public string Url { get; init; } = default!;
    public Dictionary<string, string>? Headers { get; init; }
    public DateTime ExpiresAt { get; init; }
}