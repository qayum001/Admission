using Admission.Application.Exceptions;
using Admission.Application.Services;
using LiteBus.Commands.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Admission.Application.Applicants.Commands;

public sealed record UpdateApplicantEducationLevelCommand(Guid ApplicantId, Guid EducationLevelId) : ICommand;

public sealed class UpdateApplicantEducationLevelCommandHandler(IRepository repository)
    : ICommandHandler<UpdateApplicantEducationLevelCommand>
{
    public async Task HandleAsync(UpdateApplicantEducationLevelCommand message,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var applicant = await repository.Applicants
            .Include(e => e.EducationalDocument)
            .FirstOrDefaultAsync(e => e.Id == message.ApplicantId, cancellationToken);
        
        if (applicant == null)
            throw new NotFoundException("Applicant not found");
        
        var newEducationalDocument = await repository.EducationalDocuments
            .FirstOrDefaultAsync(e => e.Id == message.EducationLevelId, cancellationToken);
        
        if (newEducationalDocument == null)
            throw new NotFoundException("Educational document not found");
        
        applicant.SetEducationalDocument(newEducationalDocument);
        await repository.SaveChangesAsync(cancellationToken);
    }
}