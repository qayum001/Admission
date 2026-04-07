namespace MailContracts;

public record AdmissionAssignedToManagerMessage(MailRecipient To, Guid Id)
    : BaseMail(To);
