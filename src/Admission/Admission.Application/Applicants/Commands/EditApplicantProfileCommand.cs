using Admission.Application.Exceptions;
using Admission.Application.Services;
using Admission.Domain.Entities;
using LiteBus.Commands.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Admission.Application.Applicants.Commands;

public sealed record EditApplicantProfileCommand(Guid ApplicantId, EditApplicantData Data) : ICommand;

public class EditApplicantProfileCommandHandler(IRepository repository) : ICommandHandler<EditApplicantProfileCommand>
{
    public async Task HandleAsync(EditApplicantProfileCommand message, CancellationToken cancellationToken = new CancellationToken())
    {
        var applicant = await repository.Applicants
            .FirstOrDefaultAsync(e => e.Id == message.ApplicantId, cancellationToken);
        
        if (applicant == null)
            throw new NotFoundException("Applicant not found");
        
        applicant.UpdateProfile(message.Data);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
