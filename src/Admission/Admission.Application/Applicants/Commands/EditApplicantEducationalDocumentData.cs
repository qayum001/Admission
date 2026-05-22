using Admission.Application.Common;
using Admission.Application.Exceptions;
using Admission.Application.Services;
using LiteBus.Commands.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Admission.Application.Applicants.Commands;

public sealed record EditApplicantEducationalDocumentDataCommand(Guid ApplicantId, Guid EducationDocumentTypeId) : ICommand;

public sealed class EditApplicantEducationalDocumentDataCommandHandler(
    IRepository repository,
    DictionaryEntityResolver resolver)
    : ICommandHandler<EditApplicantEducationalDocumentDataCommand>
{
    public async Task HandleAsync(
        EditApplicantEducationalDocumentDataCommand message,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var document = await repository.EducationalDocuments
            .FirstOrDefaultAsync(d => d.Applicant.Id == message.ApplicantId, cancellationToken);

        if (document is null)
            throw new NotFoundException("Educational document not found");

        var documentType = await resolver.GetOrCreateDocumentTypeAsync(
            message.EducationDocumentTypeId, cancellationToken);

        document.UpdateEducationDocumentType(documentType);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
