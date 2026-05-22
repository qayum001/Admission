using Admission.Application.Services;
using LiteBus.Queries.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Admission.Application.Managers.Queries;

public sealed class ManagerDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public Guid? FacultyId { get; init; }
    public string? FacultyName { get; init; }
}

public sealed record GetManagersQuery(string? Role = null, int Page = 1, int PageSize = 20)
    : IQuery<List<ManagerDto>>;

public sealed class GetManagersQueryHandler(IRepository repository)
    : IQueryHandler<GetManagersQuery, List<ManagerDto>>
{
    public async Task<List<ManagerDto>> HandleAsync(GetManagersQuery message, CancellationToken cancellationToken = default)
    {
        var page = Math.Max(1, message.Page);
        var pageSize = Math.Clamp(message.PageSize, 1, 100);

        var all = await repository.Managers
            .AsNoTracking()
            .Include(m => m.Faculty)
            .ToListAsync(cancellationToken);

        IEnumerable<Domain.Entities.Manager> filtered = all;
        if (!string.IsNullOrWhiteSpace(message.Role))
            filtered = all.Where(m => m.Role.Value.Equals(message.Role, StringComparison.OrdinalIgnoreCase));

        return filtered
            .OrderBy(m => m.Name.Value)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new ManagerDto
            {
                Id = m.Id,
                Name = m.Name.Value,
                Email = m.Email,
                Role = m.Role.Value,
                FacultyId = m.FacultyId,
                FacultyName = m.Faculty?.Name
            }).ToList();
    }
}
