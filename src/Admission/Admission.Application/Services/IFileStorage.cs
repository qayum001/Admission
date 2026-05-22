using Admission.Application.DTO;

namespace Admission.Application.Services;

public interface IFileStorage
{
    Task<PresignedUrlResult> GetDownloadUrlAsync(
        string key,
        TimeSpan expiresIn,
        CancellationToken ct = default);

    Task<PresignedUrlResult> GetUploadUrlAsync(
        string key,
        string contentType,
        TimeSpan expiresIn,
        CancellationToken ct = default);

    Task UploadAsync(
        Stream stream,
        string key,
        string contentType,
        CancellationToken ct = default);

    Task DeleteAsync(
        string key,
        CancellationToken ct = default);
}