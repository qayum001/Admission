using Admission.Application.Applicants.DTOs;
using Admission.Application.Exceptions;
using Admission.Application.Services;
using LiteBus.Queries.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Admission.Application.Applicants.Queries;

public sealed record GetApplicantEducationalDocumentQuery(Guid ApplicantId) : IQuery<EducationalDocumentDto>;

public sealed class GetApplicantEducationalDocumentQueryHandler(IRepository repository, IFileStorage fileStorage)
    : IQueryHandler<GetApplicantEducationalDocumentQuery, EducationalDocumentDto>
{
    public async Task<EducationalDocumentDto> HandleAsync(GetApplicantEducationalDocumentQuery message,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var document = await repository.EducationalDocuments
            .AsNoTracking()
            .Include(e => e.File)
            .Include(e => e.EducationDocumentType)
            .FirstOrDefaultAsync(e => e.Applicant.Id == message.ApplicantId, cancellationToken);

        if (document is null)
            throw new NotFoundException("Educational document not found");

        var scanUrl = document.File is not null
            ? await fileStorage.GetDownloadUrlAsync(document.File.Key, TimeSpan.FromMinutes(15), cancellationToken)
            : null;

        return EducationalDocumentDto.From(document, scanUrl);
    }
}
