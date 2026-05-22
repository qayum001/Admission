using Admission.Domain.ValueObjects;

namespace Admission.Domain.Entities;

public class Passport : Document
{
    public Name SerialNumber { get; private set; }
    public DateTime GivenDate { get; private set; }
    public Name GivenBy { get; private set; } 
    public Text Citizenship { get; private set; }
    public Applicant Applicant { get; private set; }

    private Passport() : base(Guid.Empty)
    {
        SerialNumber = null!;
        GivenBy = null!;
        Citizenship = null!;
        Applicant = null!;
    }
    
    public Passport(
        Guid id,
        Applicant applicant,
        File file,
        string serialNumber,
        DateTime givenDate,
        string givenBy) 
        : base(id)
    {
        Applicant = applicant;
        File = file;
        SerialNumber = new(serialNumber);
        if (givenDate >= DateTime.Now)
            throw new ArgumentException("Given date cannot be in the future");
        GivenDate = givenDate;
        GivenBy = new(givenBy);
    }

    public void EditPassportData(EditPassport data)
    {
        if (!string.IsNullOrWhiteSpace(data.NewName))
            SerialNumber = new Name(data.NewName);
        if (data.NewGivenDate.HasValue &&  data.NewGivenDate < DateTime.Now)
            GivenDate = data.NewGivenDate.Value;
        if (!string.IsNullOrWhiteSpace(data.NewGivenBy))
            GivenBy = new Name(data.NewGivenBy);
    }
}

public sealed record EditPassport(
    string? NewName,
    DateTime? NewGivenDate,
    string? NewGivenBy);
