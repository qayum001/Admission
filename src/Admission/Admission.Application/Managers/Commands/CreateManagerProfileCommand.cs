using Admission.Application.Exceptions;
using Admission.Application.Services;
using Admission.Domain.Entities;
using LiteBus.Commands.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Admission.Application.Managers.Commands;

public sealed record CreateManagerProfileCommand(
    Guid ExternalId,
    string Role,
    string Name,
    string Email) : ICommand<Guid>;

public sealed class CreateManagerProfileCommandHandler(IRepository repository)
    : ICommandHandler<CreateManagerProfileCommand, Guid>
{
    public async Task<Guid> HandleAsync(CreateManagerProfileCommand message, CancellationToken cancellationToken = default)
    {
        var exists = await repository.Managers
            .AsNoTracking()
            .AnyAsync(m => m.ExternalId == message.ExternalId, cancellationToken);

        if (exists)
            throw new InvalidActionException("Manager profile already exists");

        var manager = new Manager(Guid.NewGuid(), message.Name, message.Role, message.ExternalId, message.Email);
        repository.Managers.Add(manager);
        await repository.SaveChangesAsync(cancellationToken);

        return manager.Id;
    }
}
