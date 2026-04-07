using System.Text.Json;
using Admission.MailManager.Inbox.Handlers;
using Admission.MailManager.Inbox.Models;
using MailContracts;

namespace Admission.MailManager.Inbox.Processing;

public sealed class InboxMessageDispatcher(
    IInboxMessageHandler<NewRegistrationMessage> newRegistrationHandler,
    IInboxMessageHandler<AdmissionStatusChangedMessage> admissionStatusChangedHandler,
    IInboxMessageHandler<AdmissionAssignedToManagerMessage> admissionAssignedToManagerHandler,
    IInboxMessageHandler<AdmissionManagerAssignedToApplicantMessage> admissionManagerAssignedToApplicantHandler,
    IInboxMessageHandler<ManagerCreatedMessage> managerCreatedHandler) : IInboxMessageDispatcher
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public Task DispatchAsync(InboxMessage inboxMessage, CancellationToken cancellationToken = default)
    {
        return inboxMessage.MessageType switch
        {
            var messageType when messageType == typeof(NewRegistrationMessage).FullName =>
                newRegistrationHandler.HandleAsync(Deserialize<NewRegistrationMessage>(inboxMessage), cancellationToken),
            var messageType when messageType == typeof(AdmissionStatusChangedMessage).FullName =>
                admissionStatusChangedHandler.HandleAsync(Deserialize<AdmissionStatusChangedMessage>(inboxMessage), cancellationToken),
            var messageType when messageType == typeof(AdmissionAssignedToManagerMessage).FullName =>
                admissionAssignedToManagerHandler.HandleAsync(Deserialize<AdmissionAssignedToManagerMessage>(inboxMessage), cancellationToken),
            var messageType when messageType == typeof(AdmissionManagerAssignedToApplicantMessage).FullName =>
                admissionManagerAssignedToApplicantHandler.HandleAsync(Deserialize<AdmissionManagerAssignedToApplicantMessage>(inboxMessage), cancellationToken),
            var messageType when messageType == typeof(ManagerCreatedMessage).FullName =>
                managerCreatedHandler.HandleAsync(Deserialize<ManagerCreatedMessage>(inboxMessage), cancellationToken),
            _ => throw new InvalidOperationException($"Unsupported inbox message type '{inboxMessage.MessageType}'.")
        };
    }

    private static TMessage Deserialize<TMessage>(InboxMessage inboxMessage) where TMessage : class
    {
        var message = JsonSerializer.Deserialize<TMessage>(inboxMessage.PayloadJson, SerializerOptions);

        return message ?? throw new InvalidOperationException(
            $"Failed to deserialize inbox payload '{inboxMessage.Id}' into '{typeof(TMessage).FullName}'.");
    }
}
