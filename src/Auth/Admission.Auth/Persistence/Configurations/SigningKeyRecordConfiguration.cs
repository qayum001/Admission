using Admission.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Admission.Auth.Persistence.Configurations;

public sealed class SigningKeyRecordConfiguration : IEntityTypeConfiguration<SigningKeyRecord>
{
    public void Configure(EntityTypeBuilder<SigningKeyRecord> builder)
    {
        builder.ToTable("signing_keys");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Kid).HasMaxLength(128).IsRequired();
        builder.Property(x => x.PrivateKeyPem).IsRequired();

        builder.HasIndex(x => x.Kid).IsUnique();
        builder.HasIndex(x => x.IsActive);
    }
}
