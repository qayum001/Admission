using Quartz;

namespace Admission.MailManager.Inbox.Processing.Jobs;

[DisallowConcurrentExecution]
public sealed class InboxProcessingJob(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<InboxProcessingJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var processingService = scope.ServiceProvider.GetRequiredService<IInboxProcessingService>();

        var processedCount = await processingService.ProcessBatchAsync(context.CancellationToken);

        logger.LogDebug("Inbox processing job completed. Processed messages: {ProcessedCount}", processedCount);
    }
}
