using Admission.Application.Common;
using Admission.Application.DTO;
using Admission.Application.Exceptions;
using Admission.Application.Services;
using LiteBus.Commands.Abstractions;

namespace Admission.Application.Applicants.Commands;

public sealed record DeleteApplicantDocumentFileCommand(Guid ApplicantId, DocumentType DocType) : ICommand;

public sealed class DeleteApplicantDocumentFileCommandHandler(
    IRepository repository, IFileStorage fileStorage) : ICommandHandler<DeleteApplicantDocumentFileCommand>
{
    public async Task HandleAsync(DeleteApplicantDocumentFileCommand message,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var doc = await repository
            .GetApplicantDocumentByType(message.ApplicantId, message.DocType, cancellationToken);
        
        if (doc is null)
            throw new NotFoundException("Document not found");

        if (doc.File is null) 
            return;
        
        var file = doc.File;
        var key = file.Key;

        await fileStorage.DeleteAsync(key, cancellationToken);

        doc.SetFile(null);
        repository.Files.Remove(file);
        
        await repository.SaveChangesAsync(cancellationToken);
    }
}