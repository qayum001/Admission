namespace MailContracts;

public record AdmissionAssignedToManagerMessage(
    MailRecipient Manager,
    MailRecipient Applicant,
    Guid AdmissionId)
    : BaseMail(Manager);
