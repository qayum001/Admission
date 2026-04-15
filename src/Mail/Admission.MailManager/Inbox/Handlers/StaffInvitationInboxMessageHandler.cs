using Admission.MailManager.Mailing;
using Admission.MailManager.Templating;
using Admission.MailManager.Templating.Models;
using MailContracts;

namespace Admission.MailManager.Inbox.Handlers;

public sealed class StaffInvitationInboxMessageHandler(
    IEmailTemplateService emailTemplateService,
    IMailService mailService) : IInboxMessageHandler<StaffInvitationMessage>
{
    public Task HandleAsync(StaffInvitationMessage message, CancellationToken cancellationToken = default)
    {
        var model = new StaffInvitationTemplateModel(
            message.To.Name,
            message.RoleName,
            message.InvitationToken,
            message.FacultyId);
        var body = emailTemplateService.Render<StaffInvitationMessage>(model);

        return mailService.Send(
            message.To.Email,
            message.To.Name,
            "Staff invitation",
            body);
    }
}
