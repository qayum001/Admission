namespace Admission.MailManager.Templating.Models;

public sealed record EmailConfirmationTemplateModel(
    string ReceiverName,
    string ConfirmationToken);
