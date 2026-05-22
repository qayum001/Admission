using Admission.Application.Common;
using Admission.Application.DTO;
using Admission.Application.Exceptions;
using Admission.Application.Services;
using LiteBus.Commands.Abstractions;
using File = Admission.Domain.Entities.File;

namespace Admission.Application.Applicants.Commands;

public sealed record UpdateApplicantDocumentFileCommand(Guid ApplicantId, FileDto FileDto, DocumentType Type) : ICommand;

public sealed class UpdateApplicantDocumentFileCommandHandler(
    IRepository repository, IFileStorage fileStorage, IFileKeyGenerator fileKeyGenerator)
    : ICommandHandler<UpdateApplicantDocumentFileCommand>
{
    public async Task HandleAsync(UpdateApplicantDocumentFileCommand message,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var doc = await repository.GetApplicantDocumentByType(message.ApplicantId, message.Type, cancellationToken);

        if (doc is null)
            throw new NotFoundException("Document not found");

        var newFileKey = fileKeyGenerator
            .CreateDocumentFileKey(message.ApplicantId, doc.Id, message.FileDto.FileName);
        
        await fileStorage.UploadAsync(message.FileDto.Stream, newFileKey, message.FileDto.ContentType, cancellationToken);

        if (doc.File is not null)
        {
            var oldKey = doc.File.Key;
            
            if (!string.Equals(oldKey, newFileKey, StringComparison.Ordinal))
                await fileStorage.DeleteAsync(oldKey, cancellationToken);

            doc.File.UpdateKey(newFileKey);
            await repository.SaveChangesAsync(cancellationToken);
            return;
        }

        var newFile = new File(Guid.NewGuid(), newFileKey);
        doc.SetFile(newFile);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
