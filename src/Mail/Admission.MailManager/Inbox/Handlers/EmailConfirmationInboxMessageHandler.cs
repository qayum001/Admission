using Admission.MailManager.Mailing;
using Admission.MailManager.Templating;
using Admission.MailManager.Templating.Models;
using MailContracts;

namespace Admission.MailManager.Inbox.Handlers;

public sealed class EmailConfirmationInboxMessageHandler(
    IEmailTemplateService emailTemplateService,
    IMailService mailService) : IInboxMessageHandler<EmailConfirmationMessage>
{
    public Task HandleAsync(EmailConfirmationMessage message, CancellationToken cancellationToken = default)
    {
        var model = new EmailConfirmationTemplateModel(message.To.Name, message.ConfirmationToken);
        var body = emailTemplateService.Render<EmailConfirmationMessage>(model);

        return mailService.Send(
            message.To.Email,
            message.To.Name,
            "Email confirmation",
            body);
    }
}
