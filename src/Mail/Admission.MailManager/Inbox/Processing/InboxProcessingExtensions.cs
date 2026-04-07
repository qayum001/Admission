using Admission.MailManager.Inbox.Handlers;
using Admission.MailManager.Inbox.Processing.Jobs;
using MailContracts;
using Quartz;

namespace Admission.MailManager.Inbox.Processing;

public static class InboxProcessingExtensions
{
    public static void AddInboxProcessing(this IServiceCollection services, IConfiguration configuration)
    {
        var processingOptions = configuration.GetSection("InboxProcessing").Get<InboxProcessingOptions>() ?? new InboxProcessingOptions();

        services.Configure<InboxProcessingOptions>(configuration.GetSection("InboxProcessing"));

        services.AddScoped<IInboxMessageDispatcher, InboxMessageDispatcher>();
        services.AddScoped<IInboxProcessingService, InboxProcessingService>();

        services.AddScoped<IInboxMessageHandler<NewRegistrationMessage>, NewRegistrationInboxMessageHandler>();
        services.AddScoped<IInboxMessageHandler<AdmissionStatusChangedMessage>, AdmissionStatusChangedInboxMessageHandler>();
        services.AddScoped<IInboxMessageHandler<AdmissionAssignedToManagerMessage>, AdmissionAssignedToManagerInboxMessageHandler>();
        services.AddScoped<IInboxMessageHandler<AdmissionManagerAssignedToApplicantMessage>, AdmissionManagerAssignedToApplicantInboxMessageHandler>();
        services.AddScoped<IInboxMessageHandler<ManagerCreatedMessage>, ManagerCreatedInboxMessageHandler>();

        services.AddQuartz(quartz =>
        {
            var jobKey = new JobKey("inbox-processing");

            quartz.AddJob<InboxProcessingJob>(job => job.WithIdentity(jobKey));

            quartz.AddTrigger(trigger => trigger
                .ForJob(jobKey)
                .WithIdentity("inbox-processing-trigger")
                .StartNow()
                .WithSimpleSchedule(schedule => schedule
                    .WithInterval(TimeSpan.FromSeconds(Math.Max(1, processingOptions.PollIntervalSeconds)))
                    .RepeatForever()));
        });

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
    }
}
