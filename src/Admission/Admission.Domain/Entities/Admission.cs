using Admission.Domain.Abstractions;
using Admission.Domain.Entities.Dictionary;
using Admission.Domain.Events;
using Admission.Domain.Exceptions;

namespace Admission.Domain.Entities;

public class Admission : BaseEntity
{
    private readonly List<AdmissionProgram> _admissionProgramsList = [];

    private Admission()
    {
        Applicant = null!;
    }

    public Admission(Guid id, Applicant applicant) : base(id)
    {
        Applicant = applicant ?? throw new ArgumentNullException(nameof(applicant));
    }
    
    public AdmissionStatus AdmissionStatus { get; private set; } = AdmissionStatus.Created;
    public DateTime LastUpdatedAt { get; private set; } = DateTime.UtcNow;
    public IReadOnlyCollection<AdmissionProgram> AdmissionPrograms => _admissionProgramsList.AsReadOnly();
    public Applicant Applicant { get; private set; }
    public Manager? Manager { get; set; }

    public void UpdateAdmissionStatus(AdmissionStatus admissionStatus)
    {
        AdmissionStatus = admissionStatus;
        LastUpdatedAt = DateTime.UtcNow;
        AddEvent(new AdmissionStatusUpdateDomainEvent(this));
    }

    public AdmissionProgram AddAdnGetAdmissionProgram(EducationProgram program, int priority, int maxAdmissions)
    {
        LastUpdatedAt = DateTime.UtcNow;
        return AddAdnGetAdmissionProgramInternal(program, priority, maxAdmissions);
    }

    private AdmissionProgram AddAdnGetAdmissionProgramInternal(EducationProgram program, int priority, int maxAdmissions)
    {
        if (_admissionProgramsList.Count >= maxAdmissions)
            throw new DomainException("Maximum admissions reached");
        if (Applicant.EducationalDocument is null)
            throw new DomainException("Educational document is not set");
        
        var requiredLevel = program.EducationLevel;
        var applicantCurrentLevel = 
            Applicant.EducationalDocument.EducationDocumentType.EducationLevel;
        var availableLevels = 
            Applicant.EducationalDocument.EducationDocumentType.NextEducationLevels.Select(e => e.Id);
        
        var isCurrentLevel = applicantCurrentLevel.Id == requiredLevel.Id;
        var isNextLevel = availableLevels.Contains(requiredLevel.Id);
        if (!isCurrentLevel && !isNextLevel)
            throw new DomainException("Unavailable education level");
        
        ClearPrioritySlot(priority, maxAdmissions);
        
        var programId = Guid.NewGuid();
        var newAdmissionProgram = new AdmissionProgram(programId, priority, program,this, maxAdmissions);
        _admissionProgramsList.Add(newAdmissionProgram);
        
        NormalizePriorities(maxAdmissions);
        
        return newAdmissionProgram;
    }

    public void UpdateProgramPriority(Guid programId, int priority, int maxAdmissions)
    {
        LastUpdatedAt = DateTime.UtcNow;
        var program = _admissionProgramsList.FirstOrDefault(x => x.Id == programId);
        if (program is null)
            throw new DomainException("Program not found");
        
        _admissionProgramsList.Remove(program);
        
        ClearPrioritySlot(priority, maxAdmissions);
        program.ChangePriority(priority, maxAdmissions);
        _admissionProgramsList.Add(program);
        NormalizePriorities(maxAdmissions);
    }
    
    private void ClearPrioritySlot(int priority, int maxAdmissions)
    {
        var programsToShift = _admissionProgramsList
            .Where(x => x.Priority >= priority)
            .OrderByDescending(x => x.Priority)
            .ToList();

        foreach (var existingProgram in programsToShift)
        {
            if (existingProgram.Priority >= maxAdmissions)
                throw new DomainException("Maximum admissions reached");

            existingProgram.ChangePriority(existingProgram.Priority + 1, maxAdmissions);
        }
    }

    public void RemoveProgram(Guid id, int maxAdmissions)
    {
        LastUpdatedAt = DateTime.UtcNow;
        var program = _admissionProgramsList.FirstOrDefault(x => x.Id == id);
        if (program is null)
            throw new DomainException("Program not found");
        _admissionProgramsList.Remove(program);
        NormalizePriorities(maxAdmissions);
    }
    
    private void NormalizePriorities(int maxAdmissions)
    {
        var orderedPrograms = _admissionProgramsList
            .OrderBy(x => x.Priority)
            .ToList();

        var priority = 1;

        foreach (var program in orderedPrograms)
        {
            program.ChangePriority(priority, maxAdmissions);
            priority++;
        }
    }
}

public enum AdmissionStatus
{
    Created,
    OnReview,
    Confirmed,
    Rejected,
    Closed
}
