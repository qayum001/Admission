using Admission.Domain.Entities.Dictionary;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Admission.Persistence.Configurations;

internal sealed class EducationProgramConfiguration : IEntityTypeConfiguration<EducationProgram>
{
    public void Configure(EntityTypeBuilder<EducationProgram> builder)
    {
        builder.ToTable("education_programs");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.CreateTime)
            .HasColumnName("create_time");

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(x => x.Code)
            .HasColumnName("code")
            .HasMaxLength(64);

        builder.Property(x => x.Language)
            .HasColumnName("language")
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.EducationForm)
            .HasColumnName("education_form")
            .HasMaxLength(128)
            .IsRequired();

        builder.HasOne(x => x.Faculty)
            .WithMany()
            .HasForeignKey("faculty_id")
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne(x => x.EducationLevel)
            .WithMany()
            .HasForeignKey("education_level_id")
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasIndex(x => x.Code);
    }
}
