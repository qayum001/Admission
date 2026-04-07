using Admission.MailManager.Templating;

namespace Admission.MailManager.Mailing;

public class SmtpOptions
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string HostName { get; set; } = string.Empty;
    public string HostEmail { get; set; } = string.Empty;
    public int PoolSize { get; set; } = 2;
}

public static class SmtpExtensions
{
    public static void RegisterSmtp(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SmtpOptions>(configuration.GetSection("SmtpOptions"));
        
        services.AddScoped<IMailService, MailService>();
        services.AddSingleton<SmtpClientPool>();
        services.AddSingleton<ISmtpClientPool>(sp => sp.GetRequiredService<SmtpClientPool>());
        services.AddHostedService(sp => sp.GetRequiredService<SmtpClientPool>());
        services.AddSingleton<IEmailTemplateService, EmailTemplateService>();
    }
}