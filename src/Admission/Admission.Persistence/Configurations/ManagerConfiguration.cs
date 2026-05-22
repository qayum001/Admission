using Admission.Domain.Entities;
using Admission.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Admission.Persistence.Configurations;

internal sealed class ManagerConfiguration : IEntityTypeConfiguration<Manager>
{
    public void Configure(EntityTypeBuilder<Manager> builder)
    {
        builder.ToTable("managers");

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(256)
            .HasConversion(x => x.Value, x => new Name(x))
            .IsRequired();

        builder.Property(x => x.Email)
            .HasColumnName("email")
            .HasMaxLength(320)
            .IsRequired();

        builder.Property(x => x.FacultyId)
            .HasColumnName("faculty_id");

        builder.HasOne(x => x.Faculty)
            .WithMany()
            .HasForeignKey(x => x.FacultyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(m => m.Admissions)
            .HasField("_admissionsList")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(m => m.Admissions)
            .WithOne(nameof(Admission.Domain.Entities.Admission.Manager))
            .HasForeignKey("manager_id")
            .OnDelete(DeleteBehavior.SetNull);

        builder.Ignore(x => x.Events);
    }
}
