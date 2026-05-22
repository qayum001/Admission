namespace Admission.Application.Services;

public interface IFileValidationService
{
    void Validate(
        string fileName,
        string contentType,
        long sizeInBytes);
}
