namespace Admission.MailManager.Inbox.Processing;

public interface IInboxProcessingService
{
    Task<int> ProcessBatchAsync(CancellationToken cancellationToken = default);
}
