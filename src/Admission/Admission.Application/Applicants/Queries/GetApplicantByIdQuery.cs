using Admission.Application.Applicants.DTOs;
using Admission.Application.Exceptions;
using Admission.Application.Services;
using LiteBus.Queries.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Admission.Application.Applicants.Queries;

public sealed record GetApplicantByIdQuery(Guid Id) : IQuery<ApplicantDto>;

public sealed class GetApplicantByIdQueryHandler(IRepository repository)
    : IQueryHandler<GetApplicantByIdQuery, ApplicantDto>
{
    public async Task<ApplicantDto> HandleAsync(
        GetApplicantByIdQuery message,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var applicant = await repository.Applicants
            .AsNoTracking()
            .Include(a => a.Passport)
            .Include(a => a.EducationalDocument)
            .Include(a => a.Admission)
            .FirstOrDefaultAsync(e => e.Id == message.Id, cancellationToken);

        if (applicant is null)
            throw new NotFoundException("Applicant not found");

        return new ApplicantDto(applicant);
    }
}
