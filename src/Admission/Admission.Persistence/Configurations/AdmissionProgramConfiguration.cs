using Admission.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Admission.Persistence.Configurations;

internal sealed class AdmissionProgramConfiguration : IEntityTypeConfiguration<AdmissionProgram>
{
    public void Configure(EntityTypeBuilder<AdmissionProgram> builder)
    {
        builder.ToTable("admission_programs");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.Priority)
            .HasColumnName("priority")
            .IsRequired();

        builder.Property(x => x.ProgramId)
            .HasColumnName("program_id")
            .IsRequired();

        builder.Property(x => x.AdmissionId)
            .HasColumnName("admission_id")
            .IsRequired();

        builder.HasOne(x => x.Program)
            .WithMany()
            .HasForeignKey(x => x.ProgramId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.AdmissionId, x.Priority });

        builder.Ignore(x => x.Events);
    }
}
