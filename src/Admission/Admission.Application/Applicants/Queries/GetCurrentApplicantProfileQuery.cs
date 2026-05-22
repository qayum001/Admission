using Admission.Application.Applicants.DTOs;
using Admission.Application.Exceptions;
using Admission.Application.Services;
using LiteBus.Queries.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Admission.Application.Applicants.Queries;

public sealed record GetCurrentApplicantProfileQuery(Guid ExternalId) : IQuery<ApplicantDto>;

public sealed class GetCurrentApplicantProfileQueryHandler(IRepository repository)
    : IQueryHandler<GetCurrentApplicantProfileQuery, ApplicantDto>
{
    public async Task<ApplicantDto> HandleAsync(
        GetCurrentApplicantProfileQuery message,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var applicant = await repository.Applicants
            .AsNoTracking()
            .Include(a => a.Passport)
            .Include(a => a.EducationalDocument)
            .Include(a => a.Admission)
            .FirstOrDefaultAsync(a => a.ExternalId == message.ExternalId, cancellationToken);

        if (applicant is null)
            throw new NotFoundException("Applicant profile not found");

        return new ApplicantDto(applicant);
    }
}
