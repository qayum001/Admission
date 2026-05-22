using Admission.Application.Applicants.DTOs;
using Admission.Application.Exceptions;
using Admission.Application.Services;
using LiteBus.Commands.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Admission.Application.Applicants.Commands;

public sealed record UpdateAdmissionProgramPriorityCommand(UpdateAdmissionProgramPriorityDto Dto) : ICommand;

public sealed class UpdateAdmissionProgramPriorityCommandHandler(
    IRepository repository) : ICommandHandler<UpdateAdmissionProgramPriorityCommand>
{
    public async Task HandleAsync(UpdateAdmissionProgramPriorityCommand message,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var dto = message.Dto;
        var admission = await repository.Admissions
            .Where(e => e.Applicant.Id == dto.ApplicantId)
            .Include(e => e.AdmissionPrograms)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (admission is null)
            throw new NotFoundException("Admission not found");
        
        if (admission.AdmissionPrograms.All(e => e.Id != dto.AdmissionProgramId))
            throw new InvalidActionException("Program not found in applicant admission to update priority");
        
        admission.UpdateProgramPriority(dto.AdmissionProgramId, dto.NewPriority, dto.MaxPriority);
        
        await repository.SaveChangesAsync(cancellationToken);
    }
}