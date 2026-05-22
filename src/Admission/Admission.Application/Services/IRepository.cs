using Admission.Domain.Entities;
using Admission.Domain.Entities.Dictionary;
using Microsoft.EntityFrameworkCore;
using File = Admission.Domain.Entities.File;
using AdmissionEntity = Admission.Domain.Entities.Admission;

namespace Admission.Application.Services;

public interface IRepository
{
    DbSet<Applicant> Applicants { get; }
    DbSet<AdmissionEntity> Admissions { get; }
    DbSet<AdmissionProgram> AdmissionPrograms { get; }
    DbSet<Manager> Managers { get; }
    DbSet<Passport> Passports { get; }
    DbSet<EducationalDocument> EducationalDocuments { get; }
    DbSet<File> Files { get; }
    DbSet<Faculty> Faculties { get; }
    DbSet<EducationProgram> Programs { get; }
    DbSet<EducationLevel> EducationLevels { get; }
    DbSet<EducationDocumentType> EducationDocumentTypes { get; }
    
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken);   
}
