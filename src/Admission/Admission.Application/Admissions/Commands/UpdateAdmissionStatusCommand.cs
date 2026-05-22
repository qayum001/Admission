using Admission.Application.Exceptions;
using Admission.Application.Services;
using Admission.Domain.Entities;
using LiteBus.Commands.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Admission.Application.Admissions.Commands;

public sealed record UpdateAdmissionStatusCommand(Guid AdmissionId, AdmissionStatus NewStatus) : ICommand;

public sealed class UpdateAdmissionStatusCommandHandler(IRepository repository)
    : ICommandHandler<UpdateAdmissionStatusCommand>
{
    public async Task HandleAsync(UpdateAdmissionStatusCommand message, CancellationToken cancellationToken = default)
    {
        var admission = await repository.Admissions
            .Include(a => a.Applicant)
            .FirstOrDefaultAsync(a => a.Id == message.AdmissionId, cancellationToken);

        if (admission is null)
            throw new NotFoundException("Admission not found");

        admission.UpdateAdmissionStatus(message.NewStatus);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
