namespace MailContracts;

public record AdmissionManagerAssignedToApplicantMessage(
    MailRecipient Applicant,
    MailRecipient Manager,
    Guid AdmissionId)
    : BaseMail(Applicant);
