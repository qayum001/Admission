namespace MailContracts;

public record AdmissionStatusChangedMessage(
    MailRecipient Applicant,
    string AdmissionNumber,
    string PreviousStatusName,
    string CurrentStatusName): BaseMail(Applicant);
