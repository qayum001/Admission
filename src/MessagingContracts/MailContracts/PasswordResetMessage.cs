namespace MailContracts;

public record PasswordResetMessage(
    MailRecipient To,
    string ResetToken) : BaseMail(To);
