namespace MailContracts;

public record StaffInvitationMessage(
    MailRecipient To,
    string RoleName,
    string InvitationToken,
    Guid? FacultyId) : BaseMail(To);
