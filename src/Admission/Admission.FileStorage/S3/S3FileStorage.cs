using Admission.Application.DTO;
using Admission.Application.Services;
using Amazon.S3;
using Amazon.S3.Model;

namespace Admission.FileStorage.S3;

internal sealed class S3FileStorage(
    IAmazonS3 s3Client,
    PresigningS3Client presigningClient,
    S3FileStorageOptions options) : IFileStorage
{
    private IAmazonS3 PresignClient => presigningClient.Client;

    public async Task<PresignedUrlResult> GetDownloadUrlAsync(
        string key,
        TimeSpan expiresIn,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        ValidateKey(key);
        ValidateExpiration(expiresIn);

        var expiresAt = DateTime.UtcNow.Add(expiresIn);
        var presignedUrl = await PresignClient.GetPreSignedURLAsync(new GetPreSignedUrlRequest
        {
            BucketName = options.BucketName,
            Key = key,
            Verb = HttpVerb.GET,
            Expires = expiresAt
        });

        return new PresignedUrlResult { Url = presignedUrl, ExpiresAt = expiresAt };
    }

    public async Task<PresignedUrlResult> GetUploadUrlAsync(
        string key,
        string contentType,
        TimeSpan expiresIn,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        ValidateKey(key);
        ValidateContentType(contentType);
        ValidateExpiration(expiresIn);

        var expiresAt = DateTime.UtcNow.Add(expiresIn);
        var presignedUrl = await PresignClient.GetPreSignedURLAsync(new GetPreSignedUrlRequest
        {
            BucketName = options.BucketName,
            Key = key,
            Verb = HttpVerb.PUT,
            ContentType = contentType,
            Expires = expiresAt
        });

        return new PresignedUrlResult
        {
            Url = presignedUrl,
            Headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Content-Type"] = contentType
            },
            ExpiresAt = expiresAt
        };
    }

    public async Task UploadAsync(Stream stream, string key, string contentType, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ValidateKey(key);
        ValidateContentType(contentType);

        await s3Client.PutObjectAsync(new PutObjectRequest
        {
            BucketName = options.BucketName,
            Key = key,
            InputStream = stream,
            ContentType = contentType,
            AutoCloseStream = false,
            AutoResetStreamPosition = stream.CanSeek
        }, ct);
    }

    public async Task DeleteAsync(string key, CancellationToken ct = default)
    {
        ValidateKey(key);
        await s3Client.DeleteObjectAsync(options.BucketName, key, ct);
    }

    private static void ValidateKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("S3 object key cannot be empty.", nameof(key));
    }

    private static void ValidateContentType(string contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
            throw new ArgumentException("Content type cannot be empty.", nameof(contentType));
    }

    private static void ValidateExpiration(TimeSpan expiresIn)
    {
        if (expiresIn <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(expiresIn), "Expiration must be greater than zero.");
    }
}
