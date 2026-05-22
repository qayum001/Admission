using Admission.Application.Admissions.DTOs;
using Admission.Application.Exceptions;
using Admission.Application.Services;
using LiteBus.Queries.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Admission.Application.Admissions.Queries;

public sealed record GetApplicantAdmissionQuery(Guid ApplicantId) : IQuery<ApplicantAdmissionDto>;

public sealed class GetApplicantAdmissionQueryHandler(IRepository repository)
    : IQueryHandler<GetApplicantAdmissionQuery, ApplicantAdmissionDto>
{
    public async Task<ApplicantAdmissionDto> HandleAsync(
        GetApplicantAdmissionQuery message,
        CancellationToken cancellationToken = default)
    {
        var admission = await repository.Admissions
            .AsNoTracking()
            .Where(a => a.Applicant.Id == message.ApplicantId)
            .Include(a => a.Applicant)
            .Include(a => a.Manager)
            .Include(a => a.AdmissionPrograms)
            .ThenInclude(ap => ap.Program)
            .ThenInclude(p => p.Faculty)
            .FirstOrDefaultAsync(cancellationToken);

        if (admission is null)
            throw new NotFoundException("Admission not found");

        return new ApplicantAdmissionDto(admission);
    }
}
