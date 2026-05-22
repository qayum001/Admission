using Admission.MailManager.Mailing;
using Admission.MailManager.Templating;
using Admission.MailManager.Templating.Models;
using MailContracts;

namespace Admission.MailManager.Inbox.Handlers;

public sealed class AdmissionManagerAssignedToApplicantInboxMessageHandler(
    IEmailTemplateService emailTemplateService,
    IMailService mailService) : IInboxMessageHandler<AdmissionManagerAssignedToApplicantMessage>
{
    public Task HandleAsync(AdmissionManagerAssignedToApplicantMessage message, CancellationToken cancellationToken = default)
    {
        var model = new AdmissionManagerAssignedToApplicantTemplateModel(
            message.Applicant.Name,
            message.AdmissionId.ToString(),
            message.Manager.Name,
            message.Manager.Email);
        var body = emailTemplateService.Render<AdmissionManagerAssignedToApplicantMessage>(model);

        return mailService.Send(
            message.Applicant.Email,
            message.Applicant.Name,
            "Manager assigned",
            body);
    }
}
