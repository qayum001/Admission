using Admission.Application.Admissions.DTOs;
using Admission.Application.Exceptions;
using Admission.Application.Services;
using LiteBus.Queries.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Admission.Application.Admissions.Queries;

public sealed record GetAdmissionByIdQuery(Guid AdmissionId) : IQuery<ApplicantAdmissionDto>;

public sealed class GetAdmissionByIdQueryHandler(IRepository repository)
    : IQueryHandler<GetAdmissionByIdQuery, ApplicantAdmissionDto>
{
    public async Task<ApplicantAdmissionDto> HandleAsync(
        GetAdmissionByIdQuery message,
        CancellationToken cancellationToken = default)
    {
        var admission = await repository.Admissions
            .AsNoTracking()
            .Include(a => a.Applicant)
            .Include(a => a.Manager)
            .Include(a => a.AdmissionPrograms)
            .ThenInclude(ap => ap.Program)
            .ThenInclude(p => p.Faculty)
            .FirstOrDefaultAsync(a => a.Id == message.AdmissionId, cancellationToken);

        if (admission is null)
            throw new NotFoundException("Admission not found");

        return new ApplicantAdmissionDto(admission);
    }
}
