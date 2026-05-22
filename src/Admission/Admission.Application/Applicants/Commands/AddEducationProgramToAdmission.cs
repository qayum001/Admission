using Admission.Application.Applicants.DTOs;
using Admission.Application.Common;
using Admission.Application.Exceptions;
using Admission.Application.Services;
using Admission.Domain.Entities;
using LiteBus.Commands.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Admission.Application.Applicants.Commands;

public sealed record AddEducationProgramToAdmissionCommand(AddEducationalProgramDto Dto) : ICommand<AdmissionProgram>;

public sealed class AddEducationProgramToAdmissionCommandHandler(
    IRepository repository,
    DictionaryEntityResolver resolver)
    : ICommandHandler<AddEducationProgramToAdmissionCommand, AdmissionProgram>
{
    public async Task<AdmissionProgram> HandleAsync(AddEducationProgramToAdmissionCommand message,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var dto = message.Dto;

        var applicant = await repository.Applicants
            .Include(e => e.Admission)
            .ThenInclude(a => a!.AdmissionPrograms)
            .Include(e => e.EducationalDocument)
            .ThenInclude(d => d!.EducationDocumentType)
            .ThenInclude(t => t.EducationLevel)
            .Include(e => e.EducationalDocument)
            .ThenInclude(d => d!.EducationDocumentType)
            .ThenInclude(t => t.NextEducationLevels)
            .FirstOrDefaultAsync(e => e.Id == dto.ApplicantId, cancellationToken);

        if (applicant is null)
            throw new NotFoundException("Applicant not found");

        if (applicant.Admission is null)
        {
            var admission = new Domain.Entities.Admission(Guid.NewGuid(), applicant);
            repository.Admissions.Add(admission);
            applicant.CreateAdmission(admission);
        }
        
        var program = await resolver.GetOrCreateProgramAsync(dto.ProgramId, cancellationToken);

        var admissionProgram = applicant.Admission!.AddAdnGetAdmissionProgram(
            program, dto.Priority, dto.MaxProgramsInAdmission);

        await repository.SaveChangesAsync(cancellationToken);

        return admissionProgram;
    }
}
