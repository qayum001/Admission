using Admission.Application.Common;
using Admission.Application.DTO;
using Admission.Application.Exceptions;
using Admission.Application.Services;
using Admission.Domain.Entities;
using LiteBus.Commands.Abstractions;
using Microsoft.EntityFrameworkCore;
using File = Admission.Domain.Entities.File;

namespace Admission.Application.Applicants.Commands;

public sealed record CreateApplicantEducationalDocumentCommand(
    Guid ApplicantId,
    Guid EducationDocumentTypeId,
    FileDto File) : ICommand<Guid>;

public sealed class CreateApplicantEducationalDocumentCommandHandler(
    IRepository repository,
    IFileStorage fileStorage,
    IFileKeyGenerator fileKeyGenerator,
    DictionaryEntityResolver resolver)
    : ICommandHandler<CreateApplicantEducationalDocumentCommand, Guid>
{
    public async Task<Guid> HandleAsync(
        CreateApplicantEducationalDocumentCommand message,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var applicant = await repository.Applicants
            .Include(a => a.EducationalDocument)
            .FirstOrDefaultAsync(a => a.Id == message.ApplicantId, cancellationToken);

        if (applicant is null)
            throw new NotFoundException("Applicant not found");

        if (applicant.EducationalDocument is not null)
            throw new InvalidActionException("Educational document already exists");

        var educationDocumentType = await resolver.GetOrCreateDocumentTypeAsync(
            message.EducationDocumentTypeId, cancellationToken);

        var documentId = Guid.NewGuid();
        var fileKey = fileKeyGenerator.CreateDocumentFileKey(message.ApplicantId, documentId, message.File.FileName);
        await fileStorage.UploadAsync(message.File.Stream, fileKey, message.File.ContentType, cancellationToken);

        var documentFile = new File(Guid.NewGuid(), fileKey);
        var educationalDocument = new EducationalDocument(applicant, documentId, documentFile, educationDocumentType);

        applicant.SetEducationalDocument(educationalDocument);
        await repository.EducationalDocuments.AddAsync(educationalDocument, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return documentId;
    }
}
