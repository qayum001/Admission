using Admission.Application.Exceptions;
using Admission.Application.Services;
using Admission.Domain.Entities;
using LiteBus.Commands.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Admission.Application.Applicants.Commands;

public sealed record EditApplicantPassportCommand(Guid ApplicantId, EditPassport Data) : ICommand;

public sealed class EditApplicantPassportCommandHandler(IRepository repository) : ICommandHandler<EditApplicantPassportCommand>
{
    public async Task HandleAsync(EditApplicantPassportCommand message, CancellationToken cancellationToken = new CancellationToken())
    {
        var passport = await repository.Passports
            .FirstOrDefaultAsync(p => p.Applicant.Id == message.ApplicantId, cancellationToken);
        
        if (passport == null)
            throw new NotFoundException("Passport not found");
        
        passport.EditPassportData(message.Data);
        
        await repository.SaveChangesAsync(cancellationToken);
    }
}
