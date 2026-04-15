using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace Admission.MailManager.Mailing;

public class MailService(
    IOptions<SmtpOptions> options,
    ILogger<MailService> logger) : IMailService
{
    public async Task Send(string to, string name, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(options.Value.HostName,  options.Value.HostEmail));
        message.To.Add(new MailboxAddress(name, to));
        message.Subject = subject;
        message.Body = new TextPart(TextFormat.Html) { Text = body };

        using var client = new SmtpClient();

        try
        {
            logger.LogDebug("Sending email to {RecipientEmail} with subject {Subject}", to, subject);
            await client.ConnectAsync(options.Value.Host, options.Value.Port, false);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        catch
        {
            if (client.IsConnected)
            {
                await client.DisconnectAsync(true);
            }

            logger.LogWarning("SMTP send failed for {RecipientEmail}", to);
            throw;
        }
    }
}
