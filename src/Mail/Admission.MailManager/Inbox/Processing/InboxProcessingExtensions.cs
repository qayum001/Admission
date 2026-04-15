using Admission.MailManager.Inbox.Processing.Jobs;
using Quartz;
using System.Reflection;

namespace Admission.MailManager.Inbox.Processing;

public static class InboxProcessingExtensions
{
    public static void AddInboxProcessing(this IServiceCollection services, IConfiguration configuration)
    {
        var processingOptions = configuration.GetSection("InboxProcessing").Get<InboxProcessingOptions>() ?? new InboxProcessingOptions();

        services.Configure<InboxProcessingOptions>(configuration.GetSection("InboxProcessing"));

        services.AddScoped<IInboxMessageDispatcher, InboxMessageDispatcher>();
        services.AddScoped<IInboxProcessingService, InboxProcessingService>();

        RegisterInboxHandlers(services, typeof(InboxProcessingExtensions).Assembly);

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

    private static void RegisterInboxHandlers(IServiceCollection services, Assembly assembly)
    {
        var handlerInterfaceType = typeof(Handlers.IInboxMessageHandler<>);

        var registrations = assembly
            .GetTypes()
            .Where(type => type is { IsAbstract: false, IsInterface: false })
            .SelectMany(
                type => type.GetInterfaces()
                    .Where(@interface =>
                        @interface.IsGenericType &&
                        @interface.GetGenericTypeDefinition() == handlerInterfaceType)
                    .Select(@interface => new { ServiceType = @interface, ImplementationType = type }))
            .ToArray();

        foreach (var registration in registrations)
        {
            services.AddScoped(registration.ServiceType, registration.ImplementationType);
        }
    }
}
