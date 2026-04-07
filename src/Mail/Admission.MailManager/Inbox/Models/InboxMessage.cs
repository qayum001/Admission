namespace Admission.MailManager.Inbox.Models;

public sealed class InboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ContractMessageId { get; set; }
    public Guid? TransportMessageId { get; set; }
    public Guid? CorrelationId { get; set; }
    public Guid? ConversationId { get; set; }
    public string MessageType { get; set; } = string.Empty;
    public string PayloadJson { get; set; } = string.Empty;
    public string HeadersJson { get; set; } = "[]";
    public InboxMessageStatus Status { get; set; } = InboxMessageStatus.Pending;
    public DateTimeOffset ReceivedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? LastAttemptAtUtc { get; set; }
    public DateTimeOffset? LockedUntilUtc { get; set; }
    public DateTimeOffset? ProcessedAtUtc { get; set; }
    public int AttemptCount { get; set; }
    public string? LastError { get; set; }
    public string? SourceAddress { get; set; }
    public string? DestinationAddress { get; set; }
    public DateTimeOffset? SentAtUtc { get; set; }
}
