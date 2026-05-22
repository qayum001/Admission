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
            message.Manager.Name,
            message.AdmissionId.ToString(),
            message.Applicant.Name,
            message.Applicant.Email);
        var body = emailTemplateService.Render<AdmissionAssignedToManagerMessage>(model);

        return mailService.Send(
            message.Manager.Email,
            message.Manager.Name,
            "Admission assigned",
            body);
    }
}
