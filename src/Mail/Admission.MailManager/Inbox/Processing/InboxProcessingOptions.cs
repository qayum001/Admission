namespace Admission.MailManager.Inbox.Processing;

public sealed class InboxProcessingOptions
{
    public int BatchSize { get; set; } = 20;
    public int PollIntervalSeconds { get; set; } = 15;
    public int ProcessingLockSeconds { get; set; } = 120;
    public int MaxAttempts { get; set; } = 5;
    public int[] RetryDelaySeconds { get; set; } = [60, 300, 900, 3600, 21600];
}
