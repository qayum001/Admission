namespace MailContracts;

public record NewRegistrationMessage(MailRecipient To) 
    : BaseMail(To);