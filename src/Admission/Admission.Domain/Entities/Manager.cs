using Admission.Domain.Entities.Dictionary;
using Admission.Domain.Events;
using Admission.Domain.Exceptions;
using Admission.Domain.ValueObjects;

namespace Admission.Domain.Entities;

public class Manager : User
{
    private readonly List<Admission> _admissionsList = [];

    private Manager()
    {
    }

    public Manager(
        Guid id,
        string name,
        string role,
        Guid externalId,
        string email)
        : base(id, role, externalId)
    {
        Name = new(name);
        Email = email;
    }

    public void SetFaculty(Faculty faculty)
    {
        Faculty = faculty;
        FacultyId = faculty.Id;
    }

    public void ClearFaculty()
    {
        Faculty = null;
        FacultyId = null;
    }
    
    public Guid? FacultyId { get; private set; }
    public Faculty? Faculty { get; private set; }
    public Name Name { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public IReadOnlyCollection<Admission> Admissions => _admissionsList;

    public void AddAdmission(Admission admission)
    {
        if (_admissionsList.Contains(admission))
            throw new DomainException("Admission is already assigned to this manager");
        _admissionsList.Add(admission);
        admission.UpdateAdmissionStatus(AdmissionStatus.OnReview);
        AddEvent(new AdmissionAssignedToManagerDomainEvent(this, admission));
    }

    public void RemoveAdmission(Guid admissionId)
    {
        var admission = _admissionsList.FirstOrDefault(a => a.Id == admissionId);
        if (admission is null)
            return;
        admission.UpdateAdmissionStatus(AdmissionStatus.Created);
        _admissionsList.Remove(admission);
    }
}
