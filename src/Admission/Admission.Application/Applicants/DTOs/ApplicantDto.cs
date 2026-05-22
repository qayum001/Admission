using Admission.Domain.Entities;

namespace Admission.Application.Applicants.DTOs;

public sealed class ApplicantDto(Applicant applicant)
{
    public Guid Id { get; init; } = applicant.Id;
    public string Name { get; init; } = applicant.Name.Value;
    public DateTime BirthDate { get; init; } = applicant.BirthDate;
    public Gender Gender { get; init; } = applicant.Gender;
    public string Email { get; init; } = applicant.Email.Value;
    public string PhoneNumber { get; init; } = applicant.PhoneNumber.Value;
    public string? Citizenship { get; init; } = applicant.Citizenship;
    public Guid? PassportId { get; init; } = applicant.Passport?.Id;
    public Guid? AdmissionId { get; init; } = applicant.Admission?.Id;
    public Guid? EducationalDocumentId { get; init; } = applicant.EducationalDocument?.Id;
}
