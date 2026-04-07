namespace MailContracts;

public record AdmissionManagerAssignedToApplicantMessage(MailRecipient To, Guid AdmissionId) 
    : BaseMail(To);
