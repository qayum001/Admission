using Admission.MailManager.Inbox.Models;

namespace Admission.MailManager.Inbox.Persistence;

public interface IInboxMessageWriter
{
    Task<InboxStoreResult> StoreAsync(InboxMessage message, CancellationToken cancellationToken = default);
}
