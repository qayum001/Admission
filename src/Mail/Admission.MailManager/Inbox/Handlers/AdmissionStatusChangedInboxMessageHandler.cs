using Admission.MailManager.Mailing;
using Admission.MailManager.Templating;
using Admission.MailManager.Templating.Models;
using MailContracts;

namespace Admission.MailManager.Inbox.Handlers;

public sealed class AdmissionStatusChangedInboxMessageHandler(
    IEmailTemplateService emailTemplateService,
    IMailService mailService) : IInboxMessageHandler<AdmissionStatusChangedMessage>
{
    public Task HandleAsync(AdmissionStatusChangedMessage message, CancellationToken cancellationToken = default)
    {
        var model = new AdmissionStatusChangedTemplateModel(
            message.Applicant.Name,
            message.AdmissionNumber,
            message.PreviousStatusName,
            message.CurrentStatusName);
        var body = emailTemplateService.Render<AdmissionStatusChangedMessage>(model);

        return mailService.Send(
            message.Applicant.Email,
            message.Applicant.Name,
            "Admission status changed",
            body);
    }
}
