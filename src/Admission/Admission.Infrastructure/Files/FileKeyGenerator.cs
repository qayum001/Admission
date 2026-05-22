using System.Text.RegularExpressions;
using Admission.Application.Services;

namespace Admission.Infrastructure.Files;

internal sealed partial class FileKeyGenerator(FileKeyOptions options) : IFileKeyGenerator
{
    public string CreateDocumentFileKey(
        Guid applicantId,
        Guid documentId,
        string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be empty.", nameof(fileName));

        var extension = Path.GetExtension(fileName);
        var safeExtension = string.IsNullOrWhiteSpace(extension) ? ".bin" : NormalizeExtension(extension);
        var prefix = string.IsNullOrWhiteSpace(options.RootPrefix) ? "admission" : options.RootPrefix.Trim('/');

        return $"{prefix}/applicants/{applicantId:D}/documents/{documentId:D}/file{safeExtension}";
    }

    private static string NormalizeExtension(string extension)
    {
        var lower = extension.ToLowerInvariant();
        return AllowedExtensionPattern().IsMatch(lower) ? lower : ".bin";
    }

    [GeneratedRegex("^\\.[a-z0-9]{1,10}$")]
    private static partial Regex AllowedExtensionPattern();
}
