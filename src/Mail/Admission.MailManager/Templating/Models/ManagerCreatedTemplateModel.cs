namespace Admission.MailManager.Templating.Models;

public sealed record ManagerCreatedTemplateModel(
    string EmployeeName,
    string RoleName,
    string TemporaryPassword);
