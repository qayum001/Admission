namespace Admission.MailManager.Templating.Models;

public sealed record StaffInvitationTemplateModel(
    string ReceiverName,
    string RoleName,
    string InvitationToken,
    Guid? FacultyId);
