using Admission.MailManager.Consumers;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Admission.MailManager.Messaging;

public class MessagingOptions
{
    public string Host { get; set; } = string.Empty;
    public ushort Port { get; set; } 
    public string User { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public static class MessagingExtensions
{
    public static void AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MessagingOptions>(configuration.GetSection("MessagingParams"));
        
        services.AddMassTransit(config =>
        {
            config.AddConsumer<NewRegistrationConsumer>();
            config.AddConsumer<AdmissionStatusChangedConsumer>();
            config.AddConsumer<AdmissionManagerAssignedToApplicantConsumer>();
            config.AddConsumer<AdmissionAssignedToManagerConsumer>();
            config.AddConsumer<ManagerCreatedConsumer>();
            config.AddConsumer<StaffPasswordResetConsumer>();
            config.AddConsumer<EmailConfirmationConsumer>();
            config.AddConsumer<PasswordResetConsumer>();
            config.AddConsumer<StaffInvitationConsumer>();
            
            config.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.UseConsumeFilter(typeof(CustomRetryMiddleware<>), ctx);
        
                var options = ctx.GetRequiredService<IOptions<MessagingOptions>>().Value;
        
                cfg.Host(options.Host, options.Port, "/", host =>
                {
                    host.Username(options.User);
                    host.Password(options.Password);
                });

                cfg.ConfigureEndpoints(ctx);
            });
        });
    }
}
