using Admission.Application.DTO;
using Admission.Application.Exceptions;
using Admission.Application.Services;
using Admission.Domain.Entities;
using LiteBus.Commands.Abstractions;
using Microsoft.EntityFrameworkCore;
using File = Admission.Domain.Entities.File;

namespace Admission.Application.Applicants.Commands;

public sealed record CreateApplicantPassportCommand(
    Guid ApplicantId,
    string SerialNumber,
    DateTime GivenDate,
    string GivenBy,
    FileDto File) : ICommand<Guid>;

public sealed class CreateApplicantPassportCommandHandler(
    IRepository repository,
    IFileStorage fileStorage,
    IFileKeyGenerator fileKeyGenerator)
    : ICommandHandler<CreateApplicantPassportCommand, Guid>
{
    public async Task<Guid> HandleAsync(
        CreateApplicantPassportCommand message,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var applicant = await repository.Applicants
            .Include(a => a.Passport)
            .FirstOrDefaultAsync(a => a.Id == message.ApplicantId, cancellationToken);

        if (applicant is null)
            throw new NotFoundException("Applicant not found");

        if (applicant.Passport is not null)
            throw new InvalidActionException("Passport already exists");

        var passportId = Guid.NewGuid();
        var fileKey = fileKeyGenerator.CreateDocumentFileKey(message.ApplicantId, passportId, message.File.FileName);
        await fileStorage.UploadAsync(message.File.Stream, fileKey, message.File.ContentType, cancellationToken);

        var passportFile = new File(Guid.NewGuid(), fileKey);
        var passport = new Passport(
            passportId,
            applicant,
            passportFile,
            message.SerialNumber,
            message.GivenDate,
            message.GivenBy);

        applicant.ConfirmPassport(passport);
        await repository.Passports.AddAsync(passport, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return passportId;
    }
}
