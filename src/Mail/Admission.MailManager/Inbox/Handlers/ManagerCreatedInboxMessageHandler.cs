using Admission.MailManager.Mailing;
using Admission.MailManager.Templating;
using Admission.MailManager.Templating.Models;
using MailContracts;

namespace Admission.MailManager.Inbox.Handlers;

public sealed class ManagerCreatedInboxMessageHandler(
    IEmailTemplateService emailTemplateService,
    IMailService mailService) : IInboxMessageHandler<ManagerCreatedMessage>
{
    public Task HandleAsync(ManagerCreatedMessage message, CancellationToken cancellationToken = default)
    {
        var model = new ManagerCreatedTemplateModel(
            message.Employee.Name,
            message.RoleName,
            message.TemporaryPassword);
        var body = emailTemplateService.Render<ManagerCreatedMessage>(model);

        return mailService.Send(
            message.Employee.Email,
            message.Employee.Name,
            "Manager account created",
            body);
    }
}
