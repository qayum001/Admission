using Admission.MailManager.Mailing;
using Admission.MailManager.Templating;
using Admission.MailManager.Templating.Models;
using MailContracts;

namespace Admission.MailManager.Inbox.Handlers;

public sealed class PasswordResetInboxMessageHandler(
    IEmailTemplateService emailTemplateService,
    IMailService mailService) : IInboxMessageHandler<PasswordResetMessage>
{
    public Task HandleAsync(PasswordResetMessage message, CancellationToken cancellationToken = default)
    {
        var model = new PasswordResetTemplateModel(message.To.Name, message.ResetToken);
        var body = emailTemplateService.Render<PasswordResetMessage>(model);

        return mailService.Send(
            message.To.Email,
            message.To.Name,
            "Password reset",
            body);
    }
}
