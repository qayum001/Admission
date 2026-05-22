using Admission.Application.Exceptions;
using Admission.Application.Services;
using LiteBus.Queries.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Admission.Application.Applicants.Queries;

public sealed record GetApplicantIdByAdmissionProgramIdQuery(Guid AdmissionProgramId) : IQuery<Guid>;

public sealed class GetApplicantIdByAdmissionProgramIdQueryHandler(IRepository repository)
    : IQueryHandler<GetApplicantIdByAdmissionProgramIdQuery, Guid>
{
    public async Task<Guid> HandleAsync(GetApplicantIdByAdmissionProgramIdQuery message,
        CancellationToken cancellationToken = default)
    {
        var program = await repository.AdmissionPrograms
            .AsNoTracking()
            .Include(ap => ap.Admission)
            .ThenInclude(a => a.Applicant)
            .FirstOrDefaultAsync(ap => ap.Id == message.AdmissionProgramId, cancellationToken);

        if (program is null)
            throw new NotFoundException("Admission program not found");

        return program.Admission.Applicant.Id;
    }
}
