using Admission.MailManager.Inbox.Models;

namespace Admission.MailManager.Inbox.Processing;

public interface IInboxMessageDispatcher
{
    Task DispatchAsync(InboxMessage inboxMessage, CancellationToken cancellationToken = default);
}
