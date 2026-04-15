using Admission.Auth.Persistence;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Admission.Auth.Messaging;

public static class MessagingExtensions
{
    public static void AddAuthMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MessagingOptions>(configuration.GetSection(MessagingOptions.SectionName));
        services.AddScoped<IMailEventPublisher, MassTransitMailEventPublisher>();

        services.AddMassTransit(config =>
        {
            config.AddEntityFrameworkOutbox<AuthDbContext>(outbox =>
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
