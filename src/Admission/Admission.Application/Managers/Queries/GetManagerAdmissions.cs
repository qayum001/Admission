using Admission.Application.Services;
using LiteBus.Queries.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Admission.Application.Managers.Queries;

public sealed record GetManagerAdmissionsQuery(Guid ManagerId) : IQuery<List<AdmissionShortDto>>;

public sealed class GetManagerAdmissionsQueryHandler(IRepository repository)
    : IQueryHandler<GetManagerAdmissionsQuery, List<AdmissionShortDto>>
{
    public async Task<List<AdmissionShortDto>> HandleAsync(GetManagerAdmissionsQuery message,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var res = await repository.Admissions
            .Where(e => e.Manager != null && e.Manager.Id == message.ManagerId)
            .Include(e => e.Manager)
            .Select(e => new AdmissionShortDto(e))
            .ToListAsync(cancellationToken);

        return res;
    }
}