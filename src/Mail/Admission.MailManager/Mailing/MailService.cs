using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace Admission.MailManager.Mailing;

public class MailService(
    IOptions<SmtpOptions> options,
    ISmtpClientPool smtpClientPool,
    ILogger<MailService> logger) : IMailService
{
    public async Task Send(string to, string name, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(options.Value.HostName,  options.Value.HostEmail));
        message.To.Add(new MailboxAddress(name, to));
        message.Subject = subject;
        message.Body = new TextPart(TextFormat.Html) { Text = body };

        await using var lease = await smtpClientPool.RentAsync();

        try
        {
            logger.LogDebug("Sending email to {RecipientEmail} with subject {Subject}", to, subject);
            await lease.Client.SendAsync(message);
        }
        catch
        {
            lease.Invalidate();
            logger.LogWarning("SMTP client marked as invalid after failure while sending email to {RecipientEmail}", to);
            throw;
        }
    }
}
