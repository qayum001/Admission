using Admission.Domain.Entities;
using Admission.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Admission.Persistence.Configurations;

internal sealed class PassportConfiguration : IEntityTypeConfiguration<Passport>
{
    public void Configure(EntityTypeBuilder<Passport> builder)
    {
        builder.ToTable("passports");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.SerialNumber)
            .HasColumnName("serial_number")
            .HasMaxLength(64)
            .HasConversion(x => x.Value, x => new Name(x))
            .IsRequired();

        builder.Property(x => x.GivenDate)
            .HasColumnName("given_date")
            .IsRequired();

        builder.Property(x => x.GivenBy)
            .HasColumnName("given_by")
            .HasMaxLength(512)
            .HasConversion(x => x.Value, x => new Name(x))
            .IsRequired();

        builder.HasOne(x => x.File)
            .WithOne()
            .HasForeignKey<Passport>("file_id")
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        builder.Ignore(x => x.Citizenship);
        builder.Ignore(x => x.Events);
    }
}
