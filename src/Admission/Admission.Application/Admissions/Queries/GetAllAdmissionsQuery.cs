using Admission.Application.Admissions.DTOs;
using Admission.Application.Services;
using Admission.Domain.Entities;
using LiteBus.Queries.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Admission.Application.Admissions.Queries;

public sealed record GetAllAdmissionsQuery(
    int Page,
    int PageSize,
    string? ApplicantName,
    Guid? ProgramId,
    Guid? FacultyId,
    AdmissionStatus? Status,
    bool? WithoutManager,
    DateTime? LastUpdatedAfter,
    Guid? ManagerId = null,
    bool? SortAscending = null) : IQuery<PagedResult<AdmissionListItemDto>>;

public sealed class GetAllAdmissionsQueryHandler(IRepository repository)
    : IQueryHandler<GetAllAdmissionsQuery, PagedResult<AdmissionListItemDto>>
{
    public async Task<PagedResult<AdmissionListItemDto>> HandleAsync(
        GetAllAdmissionsQuery message,
        CancellationToken cancellationToken = default)
    {
        var query = repository.Admissions
            .AsNoTracking()
            .Include(a => a.Applicant)
            .Include(a => a.Manager)
            .Include(a => a.AdmissionPrograms)
            .ThenInclude(ap => ap.Program)
            .ThenInclude(p => p.Faculty)
            .AsQueryable();
        
        if (message.ProgramId.HasValue)
            query = query.Where(a => a.AdmissionPrograms.Any(ap => ap.ProgramId == message.ProgramId.Value));

        if (message.FacultyId.HasValue)
            query = query.Where(a => a.AdmissionPrograms.Any(ap => ap.Program.Faculty.Id == message.FacultyId.Value));

        if (message.Status.HasValue)
            query = query.Where(a => a.AdmissionStatus == message.Status.Value);

        if (message.WithoutManager == true)
            query = query.Where(a => a.Manager == null);

        if (message.LastUpdatedAfter.HasValue)
            query = query.Where(a => a.LastUpdatedAt >= message.LastUpdatedAfter.Value);

        if (message.ManagerId.HasValue)
            query = query.Where(a => a.Manager != null && a.Manager.Id == message.ManagerId.Value);

        var ordered = message.SortAscending == true
            ? query.OrderBy(a => a.LastUpdatedAt)
            : query.OrderByDescending(a => a.LastUpdatedAt);
        
        var entities = await ordered.ToListAsync(cancellationToken);

        IEnumerable<Domain.Entities.Admission> filtered = entities;
        if (!string.IsNullOrWhiteSpace(message.ApplicantName))
            filtered = entities.Where(a => a.Applicant.Name.Value.Contains(
                message.ApplicantName, StringComparison.OrdinalIgnoreCase));

        var filteredList = filtered.ToList();
        var totalCount = filteredList.Count;

        var items = filteredList
            .Skip((message.Page - 1) * message.PageSize)
            .Take(message.PageSize)
            .Select(a => new AdmissionListItemDto(a))
            .ToList();

        return new PagedResult<AdmissionListItemDto>
        {
            Items = items,
            Page = message.Page,
            PageSize = message.PageSize,
            TotalCount = totalCount
        };
    }
}
