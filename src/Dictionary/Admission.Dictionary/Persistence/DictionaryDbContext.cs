using Admission.Dictionary.Entities;
using Microsoft.EntityFrameworkCore;

namespace Admission.Dictionary.Persistence;

public class DictionaryDbContext(DbContextOptions<DictionaryDbContext> options) : DbContext (options)
{
    public DbSet<Faculty> Faculties => Set<Faculty>();
    public DbSet<EducationProgram> Programs => Set<EducationProgram>();
    public DbSet<EducationLevel> Levels => Set<EducationLevel>();
    public DbSet<EducationDocumentType> DocumentTypes => Set<EducationDocumentType>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Faculty>().HasKey(e => e.Id);
        
        modelBuilder.Entity<EducationProgram>().HasKey(e => e.Id);
        
        modelBuilder.Entity<EducationLevel>().HasKey(e => e.Id);
        
        modelBuilder.Entity<EducationDocumentType>().HasKey(e => e.Id);
        modelBuilder.Entity<EducationDocumentType>()
            .HasMany(e => e.NextEducationLevels)
            .WithOne();
    }
}