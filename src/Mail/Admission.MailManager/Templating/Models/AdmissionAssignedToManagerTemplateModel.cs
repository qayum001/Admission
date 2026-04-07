namespace Admission.MailManager.Templating.Models;

public sealed record AdmissionAssignedToManagerTemplateModel(
    string ManagerName,
    string AdmissionNumber,
    string ApplicantName,
    string ApplicantEmail);
