namespace MailContracts;

public record EmailConfirmationMessage(
    MailRecipient To,
    string ConfirmationToken) : BaseMail(To);
