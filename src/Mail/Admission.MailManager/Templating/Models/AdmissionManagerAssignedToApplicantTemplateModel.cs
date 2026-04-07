namespace Admission.MailManager.Templating.Models;

public sealed record AdmissionManagerAssignedToApplicantTemplateModel(
    string ApplicantName,
    string AdmissionNumber,
    string ManagerName,
    string ManagerEmail);
