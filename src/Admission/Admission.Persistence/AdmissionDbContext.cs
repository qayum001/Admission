using Admission.Application.Services;
using Admission.Domain.Entities;
using Admission.Domain.Entities.Dictionary;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using File = Admission.Domain.Entities.File;
using AdmissionEntity = Admission.Domain.Entities.Admission;

namespace Admission.Persistence;

public sealed class AdmissionDbContext(DbContextOptions<AdmissionDbContext> options) : DbContext(options), IRepository
{
    public DbSet<Applicant> Applicants => Set<Applicant>();
    public DbSet<AdmissionEntity> Admissions => Set<AdmissionEntity>();
    public DbSet<AdmissionProgram> AdmissionPrograms => Set<AdmissionProgram>();
    public DbSet<Manager> Managers => Set<Manager>();
    public DbSet<Passport> Passports => Set<Passport>();
    public DbSet<EducationalDocument> EducationalDocuments => Set<EducationalDocument>();
    public DbSet<File> Files => Set<File>();
    public DbSet<Faculty> Faculties => Set<Faculty>();
    public DbSet<EducationProgram> Programs => Set<EducationProgram>();
    public DbSet<EducationLevel> EducationLevels => Set<EducationLevel>();
    public DbSet<EducationDocumentType> EducationDocumentTypes => Set<EducationDocumentType>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AdmissionDbContext).Assembly);
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}
