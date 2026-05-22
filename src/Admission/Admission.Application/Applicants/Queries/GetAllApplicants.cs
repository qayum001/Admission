using Admission.Application.Applicants.DTOs;
using Admission.Application.Services;
using LiteBus.Queries.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Admission.Application.Applicants.Queries;

public sealed record GetAllApplicantsQuery() : IQuery<List<ApplicantDto>>;

public sealed class GetAllApplicantsQueryHandler(IRepository repository) : IQueryHandler<GetAllApplicantsQuery, List<ApplicantDto>>
{
    public async Task<List<ApplicantDto>> HandleAsync(GetAllApplicantsQuery message, CancellationToken cancellationToken = new CancellationToken())
    {
        var applicants = await repository.Applicants
            .Select(e => new ApplicantDto(e))
            .ToListAsync(cancellationToken);
        
        return applicants;
    }
}
