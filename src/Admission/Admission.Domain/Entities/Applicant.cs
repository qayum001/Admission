using Admission.Domain.Events;
using Admission.Domain.Exceptions;
using Admission.Domain.ValueObjects;

namespace Admission.Domain.Entities;

public class Applicant : User
{
    public Name Name { get; private set; }
    public DateTime BirthDate { get; private set; }
    public Gender Gender { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public string? Citizenship { get; private set; }
    public Admission? Admission { get; private set; }
    public Passport? Passport { get; private set; }
    public EducationalDocument? EducationalDocument { get; private set; }
    public Email Email { get; private set; }

    private Applicant() { }
    
    public Applicant(
        Guid id,
        string name,
        DateTime birthDate,
        Gender gender,
        string email,
        string role,
        Guid externalId,
        string phoneNumber,
        string? citizenship = null)
        : base(id, role, externalId)
    {
        if (!IsValidBirthDate(birthDate))
            throw new InvalidDataDomainException("Birth data should be in past");
        Name = new (name);
        BirthDate = birthDate;
        Gender = gender;
        PhoneNumber = new (phoneNumber);
        Email = new(email);
        Citizenship = citizenship;
    }

    public void CreateAdmission(Admission admission)
    {
        Admission = admission;
        AddEvent(new NewAdmissionAddedDomainEvent(this));
    }

    public void ConfirmPassport(Passport passport)
    {
        if (Passport is not null)
            throw new InvalidDataDomainException("Passport is already confirmed");
        Passport = passport ?? throw new InvalidDataDomainException("Passport should not be null");
        AddEvent(new PassportConfirmedDomainEvent(this));
    }

    public void SetEducationalDocument(EducationalDocument educationalDocument)
    {
        EducationalDocument = educationalDocument;
    }
    
    private static bool IsValidBirthDate(DateTime birthDate)
    {
        return birthDate < DateTime.Now;
    }

    public void UpdateProfile(EditApplicantData applicantData)
    {
        if (applicantData.NewName is not null)
            Name = new (applicantData.NewName);

        if (applicantData.NewBirthDate.HasValue)
        {
            if (!IsValidBirthDate(applicantData.NewBirthDate.Value))
                throw new InvalidDataDomainException("Birth data should be in past");

            BirthDate = applicantData.NewBirthDate.Value;
        }

        if (applicantData.NewGender is not null)
            Gender = applicantData.NewGender.Value;
        if (applicantData.NewPhoneNumber is not null)
            PhoneNumber = new (applicantData.NewPhoneNumber);
        if (applicantData.NewEmail is not null)
            Email = new (applicantData.NewEmail);
        if (applicantData.NewCitizenship is not null)
            Citizenship = applicantData.NewCitizenship;
    }
}

public record EditApplicantData(
    string? NewName,
    DateTime? NewBirthDate,
    Gender? NewGender,
    string? NewPhoneNumber,
    string? NewEmail,
    string? NewCitizenship = null);

public enum Gender
{
    Male,
    Female
}
