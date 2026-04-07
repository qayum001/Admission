using Admission.MailManager.Inbox.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Admission.MailManager.Inbox.Persistence.Configurations;

public sealed class InboxMessageConfiguration : IEntityTypeConfiguration<InboxMessage>
{
    public void Configure(EntityTypeBuilder<InboxMessage> builder)
    {
        builder.ToTable("inbox_messages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.ContractMessageId)
            .HasColumnName("contract_message_id")
            .IsRequired();

        builder.Property(x => x.TransportMessageId)
            .HasColumnName("transport_message_id");

        builder.Property(x => x.CorrelationId)
            .HasColumnName("correlation_id");

        builder.Property(x => x.ConversationId)
            .HasColumnName("conversation_id");

        builder.Property(x => x.MessageType)
            .HasColumnName("message_type")
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(x => x.PayloadJson)
            .HasColumnName("payload_json")
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(x => x.HeadersJson)
            .HasColumnName("headers_json")
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(x => x.ReceivedAtUtc)
            .HasColumnName("received_at_utc")
            .IsRequired();

        builder.Property(x => x.LastAttemptAtUtc)
            .HasColumnName("last_attempt_at_utc");

        builder.Property(x => x.LockedUntilUtc)
            .HasColumnName("locked_until_utc");

        builder.Property(x => x.ProcessedAtUtc)
            .HasColumnName("processed_at_utc");

        builder.Property(x => x.AttemptCount)
            .HasColumnName("attempt_count")
            .IsRequired();

        builder.Property(x => x.LastError)
            .HasColumnName("last_error");

        builder.Property(x => x.SourceAddress)
            .HasColumnName("source_address");

        builder.Property(x => x.DestinationAddress)
            .HasColumnName("destination_address");

        builder.Property(x => x.SentAtUtc)
            .HasColumnName("sent_at_utc");

        builder.HasIndex(x => x.ContractMessageId)
            .IsUnique();

        builder.HasIndex(x => new { x.Status, x.ReceivedAtUtc });
    }
}
