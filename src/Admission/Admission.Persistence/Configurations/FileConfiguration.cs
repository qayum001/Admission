using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using File = Admission.Domain.Entities.File;

namespace Admission.Persistence.Configurations;

internal sealed class FileConfiguration : IEntityTypeConfiguration<File>
{
    public void Configure(EntityTypeBuilder<File> builder)
    {
        builder.ToTable("files");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Key)
            .HasColumnName("key")
            .HasMaxLength(1024)
            .IsRequired();

        builder.HasIndex(x => x.Key).IsUnique();

        builder.Ignore(x => x.Events);
    }
}
