using Admission.Application.Exceptions;
using Admission.Application.Services;
using Admission.Domain.Entities;
using LiteBus.Queries.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Admission.Application.Applicants.Queries;

public sealed record CanUserAccessApplicantDataQuery(
    Guid ApplicantId,
    Guid UserExternalId,
    string UserRole,
    bool IsEditOperation) : IQuery<bool>;

public sealed class CanUserAccessApplicantDataQueryHandler(IRepository repository)
    : IQueryHandler<CanUserAccessApplicantDataQuery, bool>
{
    public async Task<bool> HandleAsync(
        CanUserAccessApplicantDataQuery message,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var applicant = await repository.Applicants
            .AsNoTracking()
            .Include(a => a.Admission)
            .ThenInclude(a => a!.Manager)
            .FirstOrDefaultAsync(a => a.Id == message.ApplicantId, cancellationToken);

        if (applicant is null)
            throw new NotFoundException("Applicant not found");

        if (string.Equals(message.UserRole, "Admin", StringComparison.OrdinalIgnoreCase))
            return true;

        if (string.Equals(message.UserRole, "Applicant", StringComparison.OrdinalIgnoreCase))
        {
            if (applicant.ExternalId != message.UserExternalId)
                return false;

            if (message.IsEditOperation &&
                applicant.Admission is not null &&
                applicant.Admission.AdmissionStatus == AdmissionStatus.Closed)
            {
                return false;
            }

            return true;
        }

        if (string.Equals(message.UserRole, "Manager", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(message.UserRole, "GeneralManager", StringComparison.OrdinalIgnoreCase))
        {
            return applicant.Admission?.Manager?.ExternalId == message.UserExternalId;
        }

        return false;
    }
}
