namespace Admission.MailManager.Templating.Models;

public sealed record AdmissionStatusChangedTemplateModel(
    string ApplicantName,
    string AdmissionNumber,
    string PreviousStatusName,
    string CurrentStatusName);
