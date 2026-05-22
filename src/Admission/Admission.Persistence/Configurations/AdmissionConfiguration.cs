using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AdmissionEntity = Admission.Domain.Entities.Admission;

namespace Admission.Persistence.Configurations;

internal sealed class AdmissionConfiguration : IEntityTypeConfiguration<AdmissionEntity>
{
    public void Configure(EntityTypeBuilder<AdmissionEntity> builder)
    {
        builder.ToTable("admissions");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.AdmissionStatus)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(x => x.LastUpdatedAt)
            .HasColumnName("last_updated_at")
            .IsRequired();

        builder.Navigation(x => x.AdmissionPrograms)
            .HasField("_admissionProgramsList")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.AdmissionPrograms)
            .WithOne(x => x.Admission)
            .HasForeignKey(x => x.AdmissionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(x => x.Events);
    }
}
