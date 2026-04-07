namespace MailContracts;

public record ManagerCreatedMessage(
    MailRecipient Employee,
    string RoleName,
    string TemporaryPassword) : BaseMail(Employee);
