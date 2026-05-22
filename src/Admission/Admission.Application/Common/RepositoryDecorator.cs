using Admission.Application.DTO;
using Admission.Application.Services;
using Admission.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Admission.Application.Common;

public static class RepositoryDecorator
{
    public static async Task<Document?> GetApplicantDocumentByType(this IRepository repository, Guid applicantId, DocumentType documentType, CancellationToken cancellationToken = default)
    {
        Document? doc;
        if (documentType == DocumentType.Passport)
            doc = await repository.Passports
                .Include(e => e.File)
                .Where(e => e.Applicant.Id == applicantId)
                .FirstOrDefaultAsync(cancellationToken);
        else
            doc = await repository.EducationalDocuments
                .Include(e => e.File)
                .Where(e => e.Applicant.Id == applicantId)
                .FirstOrDefaultAsync(cancellationToken);
        
        return doc;
    }
}