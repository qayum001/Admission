using Admission.Domain.Entities.Dictionary;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Admission.Persistence.Configurations;

internal sealed class EducationDocumentTypeConfiguration : IEntityTypeConfiguration<EducationDocumentType>
{
    public void Configure(EntityTypeBuilder<EducationDocumentType> builder)
    {
        builder.ToTable("education_document_types");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.CreateTime)
            .HasColumnName("create_time");

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(256)
            .IsRequired();

        builder.HasOne(x => x.EducationLevel)
            .WithMany()
            .HasForeignKey("education_level_id")
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasMany(x => x.NextEducationLevels)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "education_document_type_next_levels",
                right => right.HasOne<EducationLevel>()
                    .WithMany()
                    .HasForeignKey("next_education_level_id")
                    .OnDelete(DeleteBehavior.Restrict),
                left => left.HasOne<EducationDocumentType>()
                    .WithMany()
                    .HasForeignKey("education_document_type_id")
                    .OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.ToTable("education_document_type_next_levels");
                    join.HasKey("education_document_type_id", "next_education_level_id");
                });
    }
}
