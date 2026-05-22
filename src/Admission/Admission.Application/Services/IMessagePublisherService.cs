using Admission.Domain.Entities;

namespace Admission.Application.Services;

public interface IMessagePublisherService
{
    Task SendAdmissionStatusUpdatedMessage(string email, string name, AdmissionStatus status);

    Task SendAdmissionAssignedToManagerMessage(
        string managerEmail, string managerName,
        string applicantEmail, string applicantName,
        Guid admissionId);

    Task SendAdmissionAssignedToApplicantMessage(
        string applicantEmail, string applicantName,
        string managerEmail, string managerName,
        Guid admissionId);
}