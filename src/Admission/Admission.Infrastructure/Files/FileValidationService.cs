using Admission.Application.Services;

namespace Admission.Infrastructure.Files;

internal sealed class FileValidationService(FileValidationOptions options) : IFileValidationService
{
    private readonly HashSet<string> _allowedContentTypes = options.AllowedContentTypes
        .Select(x => x.Trim())
        .Where(x => !string.IsNullOrWhiteSpace(x))
        .ToHashSet(StringComparer.OrdinalIgnoreCase);

    public void Validate(string fileName, string contentType, long sizeInBytes)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be empty.", nameof(fileName));

        if (string.IsNullOrWhiteSpace(contentType))
            throw new ArgumentException("Content type cannot be empty.", nameof(contentType));

        if (sizeInBytes <= 0)
            throw new ArgumentOutOfRangeException(nameof(sizeInBytes), "File size must be greater than zero.");

        if (sizeInBytes > options.MaxSizeBytes)
            throw new InvalidOperationException($"File size exceeds limit ({options.MaxSizeBytes} bytes).");

        if (_allowedContentTypes.Count > 0 && !_allowedContentTypes.Contains(contentType))
            throw new InvalidOperationException($"Content type '{contentType}' is not allowed.");
    }
}
