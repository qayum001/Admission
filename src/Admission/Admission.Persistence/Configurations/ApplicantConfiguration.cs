using Admission.Domain.Entities;
using Admission.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Admission.Persistence.Configurations;

internal sealed class ApplicantConfiguration : IEntityTypeConfiguration<Applicant>
{
    public void Configure(EntityTypeBuilder<Applicant> builder)
    {
        builder.ToTable("applicants");

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(256)
            .HasConversion(x => x.Value, x => new Name(x))
            .IsRequired();

        builder.Property(x => x.BirthDate)
            .HasColumnName("birth_date")
            .IsRequired();

        builder.Property(x => x.Gender)
            .HasColumnName("gender")
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(x => x.PhoneNumber)
            .HasColumnName("phone_number")
            .HasMaxLength(32)
            .HasConversion(x => x.Value, x => new PhoneNumber(x))
            .IsRequired();

        builder.Property(x => x.Email)
            .HasColumnName("email")
            .HasMaxLength(320)
            .HasConversion(x => x.Value, x => new Email(x))
            .IsRequired();

        builder.Property(x => x.Citizenship)
            .HasColumnName("citizenship")
            .HasMaxLength(128)
            .IsRequired(false);

        builder.HasOne(x => x.Admission)
            .WithOne(x => x.Applicant)
            .HasForeignKey<Admission.Domain.Entities.Admission>("applicant_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Passport)
            .WithOne(x => x.Applicant)
            .HasForeignKey<Passport>("applicant_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.EducationalDocument)
            .WithOne(x => x.Applicant)
            .HasForeignKey<EducationalDocument>("applicant_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(x => x.Events);
    }
}
