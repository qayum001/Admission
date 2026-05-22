using Admission.Domain.Entities;
using Admission.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Admission.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.UseTpcMappingStrategy();

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Role)
            .HasColumnName("role")
            .HasMaxLength(64)
            .HasConversion(x => x.Value, x => new Text(x))
            .IsRequired();

        builder.Property(x => x.ExternalId)
            .HasColumnName("external_id")
            .IsRequired();
    }
}
