using Admission.Domain.Entities.Dictionary;

namespace Admission.Domain.Entities;

public class EducationalDocument : Document
{
    public EducationDocumentType EducationDocumentType { get; private set; }
    public Applicant Applicant { get; private set; }

    private EducationalDocument() : base(Guid.Empty)
    {
        EducationDocumentType = null!;
        Applicant = null!;
    }

    public EducationalDocument(
        Applicant applicant,
        Guid id,
        File? file,
        EducationDocumentType educationDocumentType)
        : base(id)
    {
        Applicant = applicant;
        File = file;
        EducationDocumentType = educationDocumentType;
    }

    public void UpdateEducationDocumentType(EducationDocumentType educationDocumentType)
    {
        EducationDocumentType = educationDocumentType ?? throw new ArgumentNullException(nameof(educationDocumentType));
    }
}
