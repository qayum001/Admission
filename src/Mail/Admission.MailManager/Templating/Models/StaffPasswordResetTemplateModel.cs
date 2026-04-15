namespace Admission.MailManager.Templating.Models;

public sealed record StaffPasswordResetTemplateModel(
    string EmployeeName,
    string RoleName,
    string TemporaryPassword);
