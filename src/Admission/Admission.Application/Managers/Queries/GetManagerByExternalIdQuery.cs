using Admission.Application.Exceptions;
using Admission.Application.Services;
using LiteBus.Queries.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Admission.Application.Managers.Queries;

public sealed record GetManagerByExternalIdQuery(Guid ExternalId) : IQuery<Guid>;

public sealed class GetManagerByExternalIdQueryHandler(IRepository repository)
    : IQueryHandler<GetManagerByExternalIdQuery, Guid>
{
    public async Task<Guid> HandleAsync(GetManagerByExternalIdQuery message, CancellationToken cancellationToken = default)
    {
        var manager = await repository.Managers
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.ExternalId == message.ExternalId, cancellationToken);

        if (manager is null)
            throw new NotFoundException("Manager profile not found");

        return manager.Id;
    }
}
