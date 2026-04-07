using Admission.MailManager.Mailing;
using Admission.MailManager.Templating;
using Admission.MailManager.Templating.Models;
using MailContracts;

namespace Admission.MailManager.Inbox.Handlers;

public sealed class NewRegistrationInboxMessageHandler(
    IEmailTemplateService emailTemplateService,
    IMailService mailService) : IInboxMessageHandler<NewRegistrationMessage>
{
    public Task HandleAsync(NewRegistrationMessage message, CancellationToken cancellationToken = default)
    {
        var model = new NewRegistrationTemplateModel(message.To.Name, message.To.Email);
        var body = emailTemplateService.Render<NewRegistrationMessage>(model);

        return mailService.Send(message.To.Email, message.To.Name, "New registration", body);
    }
}
