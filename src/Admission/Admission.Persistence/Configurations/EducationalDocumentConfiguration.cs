using Admission.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Admission.Persistence.Configurations;

internal sealed class EducationalDocumentConfiguration : IEntityTypeConfiguration<EducationalDocument>
{
    public void Configure(EntityTypeBuilder<EducationalDocument> builder)
    {
        builder.ToTable("educational_documents");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.HasOne(x => x.File)
            .WithOne()
            .HasForeignKey<EducationalDocument>("file_id")
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        builder.HasOne(x => x.EducationDocumentType)
            .WithMany()
            .HasForeignKey("education_document_type_id")
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Ignore(x => x.Events);
    }
}
