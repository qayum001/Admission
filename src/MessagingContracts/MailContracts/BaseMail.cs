namespace MailContracts;

public abstract record BaseMail(MailRecipient To) : IInboxMessage
{
    public Guid ContractMessageId { get; init; } = Guid.NewGuid();
    public DateTimeOffset CreatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
}
