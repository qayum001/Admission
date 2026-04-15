using Admission.MailManager.Mailing;
using Admission.MailManager.Templating;
using Admission.MailManager.Templating.Models;
using MailContracts;

namespace Admission.MailManager.Inbox.Handlers;

public sealed class StaffPasswordResetInboxMessageHandler(
    IEmailTemplateService emailTemplateService,
    IMailService mailService) : IInboxMessageHandler<StaffPasswordResetMessage>
{
    public Task HandleAsync(StaffPasswordResetMessage message, CancellationToken cancellationToken = default)
    {
        var model = new StaffPasswordResetTemplateModel(
            message.Employee.Name,
            message.RoleName,
            message.TemporaryPassword);
        var body = emailTemplateService.Render<StaffPasswordResetMessage>(model);

        return mailService.Send(
            message.Employee.Email,
            message.Employee.Name,
            "Staff password reset",
            body);
    }
}
