using Admission.Application.Exceptions;
using Admission.Application.Services;
using LiteBus.Commands.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Admission.Application.Managers.Commands;

public sealed record DetachAdmissionFromManagerCommand(Guid ManagerId, Guid AdmissionId) : ICommand;

public sealed class DetachAdmissionFromManagerCommandHandler(IRepository repository) : ICommandHandler<DetachAdmissionFromManagerCommand>
{
    public async Task HandleAsync(DetachAdmissionFromManagerCommand message,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var manager = await repository.Managers
            .Include(e => e.Admissions)
            .ThenInclude(a => a.Applicant)
            .FirstOrDefaultAsync(e => e.Id == message.ManagerId, cancellationToken);
        
        if (manager == null)
            throw new NotFoundException("Manager not found");
        
        manager.RemoveAdmission(message.AdmissionId);
        await repository.SaveChangesAsync(cancellationToken);
    }
}