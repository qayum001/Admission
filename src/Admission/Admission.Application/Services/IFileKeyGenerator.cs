namespace Admission.Application.Services;

public interface IFileKeyGenerator
{
    string CreateDocumentFileKey(
        Guid applicantId,
        Guid documentId,
        string fileName);
}
