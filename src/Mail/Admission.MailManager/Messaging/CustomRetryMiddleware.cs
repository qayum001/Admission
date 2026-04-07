using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Admission.MailManager.Inbox.Models;
using Admission.MailManager.Inbox.Persistence;
using MailContracts;
using MassTransit;

namespace Admission.MailManager.Messaging;

[SuppressMessage("Performance", "CA1873:Avoid potentially expensive logging")]
public class CustomRetryMiddleware<T>(
    ILogger<CustomRetryMiddleware<T>> logger,
    IInboxMessageWriter inboxMessageWriter)
    : IFilter<ConsumeContext<T>> where T : class
{
    // ReSharper disable once StaticMemberInGenericType
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        logger.LogDebug("{CustomRetryMiddleware} captured message {MessageType}", nameof(CustomRetryMiddleware<>), typeof(T).Name);

        var inboxMessage = CreateInboxMessage(context);
        var result = await inboxMessageWriter.StoreAsync(inboxMessage, context.CancellationToken);

        if (result == InboxStoreResult.Duplicate)
        {
            logger.LogInformation(
                "Skipped duplicate inbox message {ContractMessageId} for {MessageType}",
                inboxMessage.ContractMessageId,
                inboxMessage.MessageType);

            return;
        }

        logger.LogInformation(
            "Stored inbox message {ContractMessageId} for {MessageType}",
            inboxMessage.ContractMessageId,
            inboxMessage.MessageType);
    }

    public void Probe(ProbeContext context) {}

    private static InboxMessage CreateInboxMessage(ConsumeContext<T> context)
    {
        if (context.Message is not IInboxMessage inboxContract)
        {
            throw new InvalidOperationException(
                $"Message type '{typeof(T).FullName}' must implement '{nameof(IInboxMessage)}' to be stored in inbox.");
        }

        var headers = context.Headers.GetAll()
            .Select(x => new InboxMessageHeader(x.Key, x.Value?.ToString() ?? string.Empty))
            .ToArray();

        return new InboxMessage
        {
            ContractMessageId = inboxContract.ContractMessageId,
            TransportMessageId = context.MessageId,
            CorrelationId = context.CorrelationId,
            ConversationId = context.ConversationId,
            MessageType = typeof(T).FullName ?? typeof(T).Name,
            PayloadJson = JsonSerializer.Serialize(context.Message, SerializerOptions),
            HeadersJson = JsonSerializer.Serialize(headers, SerializerOptions),
            Status = InboxMessageStatus.Pending,
            ReceivedAtUtc = DateTimeOffset.UtcNow,
            AttemptCount = 0,
            SourceAddress = context.SourceAddress?.ToString(),
            DestinationAddress = context.DestinationAddress?.ToString(),
            SentAtUtc = context.SentTime.HasValue ? new DateTimeOffset(context.SentTime.Value) : null
        };
    }
}
