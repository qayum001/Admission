namespace Admission.MailManager.Templating.Models;

public sealed record PasswordResetTemplateModel(
    string ReceiverName,
    string ResetToken);
