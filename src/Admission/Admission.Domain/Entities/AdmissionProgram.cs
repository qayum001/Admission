using Admission.Domain.Abstractions;
using Admission.Domain.Entities.Dictionary;
using Admission.Domain.Exceptions;

namespace Admission.Domain.Entities;

public class AdmissionProgram : BaseEntity
{
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public int Priority { get; private set; }
    public Guid ProgramId { get; private set; }
    public EducationProgram Program { get; private set; }
    public Guid AdmissionId { get; private set; }
    public Admission Admission { get; private set; }

    private AdmissionProgram()
    {
        Program = null!;
        Admission = null!;
    }
    
    public AdmissionProgram(
        Guid id,
        int priority,
        EducationProgram program,
        Admission admission,
        int maxAdmissions)
        : base(id)
    {
        if (priority < 1 || priority > maxAdmissions)
            throw new InvalidDataDomainException($"Priority must be in range [1, {maxAdmissions}]");
        Priority = priority;
        Program = program;
        ProgramId = program.Id;
        Admission = admission;
        AdmissionId = admission.Id;
    }
    
    public void ChangePriority(int priority, int maxAdmissions)
    {
        if (priority < 1 || priority > maxAdmissions)
            throw new InvalidDataDomainException($"Priority must be in range [1, {maxAdmissions}]");
        Priority = priority;
    }
}
