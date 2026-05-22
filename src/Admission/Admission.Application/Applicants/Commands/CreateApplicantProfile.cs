using Admission.Application.Applicants.DTOs;
using Admission.Application.Exceptions;
using Admission.Application.Services;
using Admission.Domain.Entities;
using Admission.Domain.ValueObjects;
using LiteBus.Commands.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Admission.Application.Applicants.Commands;

public sealed record CreateApplicantProfileCommand(
    Guid ExternalId,
    string Role,
    string Email,
    CreateApplicantProfileDto Dto) : ICommand<Guid>;

public sealed class CreateApplicantProfileCommandHandler(IRepository repository)
    : ICommandHandler<CreateApplicantProfileCommand, Guid>
{
    public async Task<Guid> HandleAsync(
        CreateApplicantProfileCommand message,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var hasApplicantWithExternalId = await repository.Applicants
            .AsNoTracking()
            .AnyAsync(a => a.ExternalId == message.ExternalId, cancellationToken);

        if (hasApplicantWithExternalId)
            throw new InvalidActionException("Applicant profile already exists");

        var hasApplicantWithEmail = await repository.Applicants
            .AsNoTracking()
            .AnyAsync(a => a.Email == new Email(message.Email), cancellationToken);

        if (hasApplicantWithEmail)
            throw new InvalidActionException("Email already exists");

        var applicantId = Guid.NewGuid();
        var dto = message.Dto;

        var applicant = new Applicant(
            applicantId,
            dto.Name,
            dto.BirthDate,
            dto.Gender,
            message.Email,
            message.Role,
            message.ExternalId,
            dto.PhoneNumber,
            dto.Citizenship);

        await repository.Applicants.AddAsync(applicant, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return applicantId;
    }
}
