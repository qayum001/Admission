using Admission.MailManager.Mailing;
using Admission.MailManager.Templating;
using Admission.MailManager.Templating.Models;
using MailContracts;

namespace Admission.MailManager.Inbox.Handlers;

public sealed class AdmissionAssignedToManagerInboxMessageHandler(
    IEmailTemplateService emailTemplateService,
    IMailService mailService) : IInboxMessageHandler<AdmissionAssignedToManagerMessage>
{
    public Task HandleAsync(AdmissionAssignedToManagerMessage message, CancellationToken cancellationToken = default)
    {
        var model = new AdmissionAssignedToManagerTemplateModel(
            message.To.Name,
            message.Id.ToString(),
            message.To.Name,
            message.To.Email);
        var body = emailTemplateService.Render<AdmissionAssignedToManagerMessage>(model);

        return mailService.Send(
            message.To.Email,
            message.To.Name,
            "Admission assigned",
            body);
    }
}
