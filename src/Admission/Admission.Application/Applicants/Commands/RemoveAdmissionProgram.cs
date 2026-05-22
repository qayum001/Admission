using Admission.Application.Exceptions;
using Admission.Application.Services;
using LiteBus.Commands.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Admission.Application.Applicants.Commands;

public sealed record RemoveAdmissionProgramCommand(Guid ApplicantId, Guid ProgramId, int MaxPrograms) : ICommand;

public sealed class RemoveAdmissionProgramCommandHandler(IRepository repository)
    : ICommandHandler<RemoveAdmissionProgramCommand>
{
    public async Task HandleAsync(RemoveAdmissionProgramCommand message, CancellationToken cancellationToken = new CancellationToken())
    {
        var admission = await repository.Admissions
            .Where(e => e.Applicant.Id == message.ApplicantId)
            .Include(e => e.AdmissionPrograms)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (admission is null)
            throw new NotFoundException("Admission not found");
                
        var admissionProgram = admission.AdmissionPrograms.FirstOrDefault(e=> e.Id == message.ProgramId);
        
        admission.RemoveProgram(message.ProgramId, message.MaxPrograms);

        if (admissionProgram is not null)
            repository.AdmissionPrograms.Remove(admissionProgram);
        
        await repository.SaveChangesAsync(cancellationToken);
    }
}