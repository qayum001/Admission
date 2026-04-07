using Admission.MailManager.Inbox.Models;
using Admission.MailManager.Inbox.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Admission.MailManager.Inbox.Processing;

public sealed class InboxProcessingService(
    InboxDbContext dbContext,
    IInboxMessageDispatcher inboxMessageDispatcher,
    IOptions<InboxProcessingOptions> options,
    ILogger<InboxProcessingService> logger) : IInboxProcessingService
{
    private readonly InboxProcessingOptions _options = options.Value;

    public async Task<int> ProcessBatchAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;

        var ids = await dbContext.InboxMessages
            .AsNoTracking()
            .Where(x =>
                (x.Status == InboxMessageStatus.Pending ||
                 x.Status == InboxMessageStatus.Failed ||
                 x.Status == InboxMessageStatus.Processing) &&
                (x.LockedUntilUtc == null || x.LockedUntilUtc <= now))
            .OrderBy(x => x.ReceivedAtUtc)
            .Select(x => x.Id)
            .Take(_options.BatchSize)
            .ToListAsync(cancellationToken);

        var processedCount = 0;

        foreach (var id in ids)
        {
            if (await ProcessSingleMessageAsync(id, cancellationToken))
            {
                processedCount++;
            }
        }

        return processedCount;
    }

    private async Task<bool> ProcessSingleMessageAsync(Guid id, CancellationToken cancellationToken)
    {
        var claimedAt = DateTimeOffset.UtcNow;
        var inboxMessage = await dbContext.InboxMessages
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (inboxMessage is null || !CanBeClaimed(inboxMessage, claimedAt))
        {
            return false;
        }

        inboxMessage.Status = InboxMessageStatus.Processing;
        inboxMessage.AttemptCount++;
        inboxMessage.LastAttemptAtUtc = claimedAt;
        inboxMessage.LockedUntilUtc = claimedAt.AddSeconds(_options.ProcessingLockSeconds);

        await dbContext.SaveChangesAsync(cancellationToken);

        try
        {
            await inboxMessageDispatcher.DispatchAsync(inboxMessage, cancellationToken);

            inboxMessage.Status = InboxMessageStatus.Processed;
            inboxMessage.ProcessedAtUtc = DateTimeOffset.UtcNow;
            inboxMessage.LockedUntilUtc = null;
            inboxMessage.LastError = null;

            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Processed inbox message {ContractMessageId} of type {MessageType}",
                inboxMessage.ContractMessageId,
                inboxMessage.MessageType);

            return true;
        }
        catch (Exception ex)
        {
            inboxMessage.LastError = ex.ToString();

            if (inboxMessage.AttemptCount >= _options.MaxAttempts)
            {
                inboxMessage.Status = InboxMessageStatus.Poisoned;
                inboxMessage.LockedUntilUtc = null;
            }
            else
            {
                inboxMessage.Status = InboxMessageStatus.Failed;
                inboxMessage.LockedUntilUtc = DateTimeOffset.UtcNow.Add(GetRetryDelay(inboxMessage.AttemptCount));
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogError(
                ex,
                "Failed to process inbox message {ContractMessageId} of type {MessageType}. Attempt {AttemptCount}",
                inboxMessage.ContractMessageId,
                inboxMessage.MessageType,
                inboxMessage.AttemptCount);

            return false;
        }
    }

    private static bool CanBeClaimed(InboxMessage inboxMessage, DateTimeOffset now)
    {
        return (inboxMessage.Status == InboxMessageStatus.Pending ||
                inboxMessage.Status == InboxMessageStatus.Failed ||
                inboxMessage.Status == InboxMessageStatus.Processing) &&
               (inboxMessage.LockedUntilUtc == null || inboxMessage.LockedUntilUtc <= now);
    }

    private TimeSpan GetRetryDelay(int attemptCount)
    {
        if (_options.RetryDelaySeconds.Length == 0)
        {
            return TimeSpan.FromMinutes(5);
        }

        var index = Math.Clamp(attemptCount - 1, 0, _options.RetryDelaySeconds.Length - 1);
        return TimeSpan.FromSeconds(_options.RetryDelaySeconds[index]);
    }
}
