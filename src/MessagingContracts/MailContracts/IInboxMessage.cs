namespace MailContracts;

public interface IInboxMessage
{
    Guid ContractMessageId { get; init; }
    DateTimeOffset CreatedAtUtc { get; init; }
}
