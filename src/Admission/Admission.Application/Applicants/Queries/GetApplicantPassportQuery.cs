using Admission.Application.Applicants.DTOs;
using Admission.Application.Exceptions;
using Admission.Application.Services;
using LiteBus.Queries.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Admission.Application.Applicants.Queries;

public sealed record GetApplicantPassportQuery(Guid ApplicantId) : IQuery<PassportDto>;

public sealed class GetApplicantPassportQueryHandler(IRepository repository, IFileStorage fileStorage)
    : IQueryHandler<GetApplicantPassportQuery, PassportDto>
{
    public async Task<PassportDto> HandleAsync(GetApplicantPassportQuery message,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var passport = await repository.Passports
            .AsNoTracking()
            .Include(e => e.File)
            .FirstOrDefaultAsync(e => e.Applicant.Id == message.ApplicantId, cancellationToken);

        if (passport is null)
            throw new NotFoundException("Passport not found");

        var scanUrl = passport.File is not null
            ? await fileStorage.GetDownloadUrlAsync(passport.File.Key, TimeSpan.FromMinutes(15), cancellationToken)
            : null;

        return PassportDto.From(passport, scanUrl);
    }
}
