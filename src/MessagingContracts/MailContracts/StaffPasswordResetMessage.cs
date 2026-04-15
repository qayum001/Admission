namespace MailContracts;

public record StaffPasswordResetMessage(
    MailRecipient Employee,
    string RoleName,
    string TemporaryPassword) : BaseMail(Employee);
