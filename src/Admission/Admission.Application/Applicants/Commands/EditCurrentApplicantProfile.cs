using Admission.Application.Exceptions;
using Admission.Application.Services;
using Admission.Domain.Entities;
using Admission.Domain.ValueObjects;
using LiteBus.Commands.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Admission.Application.Applicants.Commands;

public sealed record EditCurrentApplicantProfileCommand(Guid ExternalId, EditApplicantData Data) : ICommand;

public sealed class EditCurrentApplicantProfileCommandHandler(IRepository repository)
    : ICommandHandler<EditCurrentApplicantProfileCommand>
{
    public async Task HandleAsync(
        EditCurrentApplicantProfileCommand message,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var applicant = await repository.Applicants
            .FirstOrDefaultAsync(a => a.ExternalId == message.ExternalId, cancellationToken);

        if (applicant is null)
            throw new NotFoundException("Applicant profile not found");

        if (!string.IsNullOrWhiteSpace(message.Data.NewEmail))
        {
            var hasDuplicateEmail = await repository.Applicants
                .AsNoTracking()
                .AnyAsync(
                    a => a.Id != applicant.Id && a.Email == new Email(message.Data.NewEmail),
                    cancellationToken);

            if (hasDuplicateEmail)
                throw new InvalidActionException("Email already exists");
        }

        applicant.UpdateProfile(message.Data);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
