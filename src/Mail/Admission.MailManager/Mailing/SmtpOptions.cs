using Admission.MailManager.Templating;

namespace Admission.MailManager.Mailing;

public class SmtpOptions
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string HostName { get; set; } = string.Empty;
    public string HostEmail { get; set; } = string.Empty;
}

public static class SmtpExtensions
{
    public static void RegisterSmtp(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SmtpOptions>(configuration.GetSection("SmtpOptions"));
        
        services.AddScoped<IMailService, MailService>();
        services.AddSingleton<IEmailTemplateService, EmailTemplateService>();
    }
}
