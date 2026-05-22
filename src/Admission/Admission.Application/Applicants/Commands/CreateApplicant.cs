using Admission.Application.Applicants.DTOs;
using Admission.Application.Exceptions;
using Admission.Application.Services;
using Admission.Domain.Entities;
using Admission.Domain.ValueObjects;
using LiteBus.Commands.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Admission.Application.Applicants.Commands;

public sealed record CreateApplicantCommand(CreateApplicantDto Dto) : ICommand<Guid>;

public sealed class CreateApplicantCommandHandler(IRepository repository) 
    : ICommandHandler<CreateApplicantCommand, Guid>
{
    public async Task<Guid> HandleAsync(CreateApplicantCommand message, CancellationToken cancellationToken = new CancellationToken())
    {
        var applicantId = Guid.NewGuid();
        var dto = message.Dto;
        
        var applicantWithEmail = await repository.Applicants
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Email == new Email(dto.Email), cancellationToken);
        
        if (applicantWithEmail != null)
            throw new InvalidActionException("Email already exists");
        
        var applicant = new Applicant(
            applicantId,
            dto.Name,
            dto.BirthDate,
            dto.Gender,
            dto.Email,
            "Applicant",
            applicantId,
            dto.PhoneNumber);
        
        await repository.Applicants.AddAsync(applicant, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return applicantId;
    }
}
