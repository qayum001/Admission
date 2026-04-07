namespace Admission.MailManager.Inbox.Handlers;

public interface IInboxMessageHandler<in TMessage> where TMessage : class
{
    Task HandleAsync(TMessage message, CancellationToken cancellationToken = default);
}
