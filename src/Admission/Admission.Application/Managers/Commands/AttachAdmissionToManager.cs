using Admission.Application.Exceptions;
using Admission.Application.Services;
using LiteBus.Commands.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Admission.Application.Managers.Commands;

public sealed record AttachAdmissionToManagerCommand(Guid ManagerId, Guid AdmissionId) : ICommand;

public sealed class AttachAdmissionToManagerCommandHandler(IRepository repository) 
    : ICommandHandler<AttachAdmissionToManagerCommand>
{
    public async Task HandleAsync(AttachAdmissionToManagerCommand message, CancellationToken ct = new CancellationToken())
    {
        var manager = await repository.Managers
            .Include(e => e.Admissions)
            .Include(e => e.Faculty)
            .FirstOrDefaultAsync(e => e.Id == message.ManagerId, ct);
        
        if (manager == null)
            throw new NotFoundException("Manager not found");
        
        if (manager.Faculty is null)
            throw new InvalidActionException("Manager should be attached to faculty");

        if (manager.Admissions.Any(a => a.Id == message.AdmissionId))
            throw new InvalidActionException("Admission is already assigned to this manager");

        var admission = await repository.Admissions
            .Include(e => e.Applicant)
            .Include(e => e.Manager)
            .Include(e => e.AdmissionPrograms)
            .ThenInclude(e => e.Program)
            .ThenInclude(e => e.Faculty)
            .FirstOrDefaultAsync(e => e.Id == message.AdmissionId, ct);

        if (admission is null)
            throw new NotFoundException("Admission not found");

        if (admission.Manager is not null)
            throw new InvalidActionException("Admission already has a manager assigned. Detach it first.");
        
        var admissionProgramIds = admission.AdmissionPrograms.Select(a => a.ProgramId).ToList();

        var hasAdmissionManagersFacultyProgram = await repository.Programs
            .AnyAsync(e => admissionProgramIds.Contains(e.Id) && e.Faculty.Id == manager.FacultyId, ct);
        
        if (!hasAdmissionManagersFacultyProgram)
            throw new InvalidActionException("Admission should contain at least one Manager's faculty program");
        
        manager.AddAdmission(admission);
        await repository.SaveChangesAsync(ct);
    }
}