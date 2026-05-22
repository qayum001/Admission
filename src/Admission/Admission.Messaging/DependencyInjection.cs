using Admission.Application.Services;
using Admission.Persistence;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Admission.Messaging;

public static class DependencyInjection
{
    public static void AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MessagingOptions>(configuration.GetSection(MessagingOptions.SectionName));
        services.AddScoped<IMessagePublisherService, MassTransitAdmissionEventsPublisher>();

        services.AddMassTransit(config =>
        {
            config.AddEntityFrameworkOutbox<AdmissionDbContext>(outbox =>
            {
                outbox.UsePostgres();
                outbox.UseBusOutbox();
            });

            config.UsingRabbitMq((ctx, cfg) =>
            {
                var options = ctx.GetRequiredService<IOptions<MessagingOptions>>().Value;

                cfg.Host(options.Host, options.Port, "/", host =>
                {
                    host.Username(options.User);
                    host.Password(options.Password);
                });
            });
        });
    }
}