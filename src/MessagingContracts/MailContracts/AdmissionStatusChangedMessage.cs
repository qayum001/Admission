namespace MailContracts;

public record AdmissionStatusChangedMessage(
    MailRecipient Applicant,
    string CurrentStatusName): BaseMail(Applicant);
