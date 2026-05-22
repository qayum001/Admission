using Admission.Application.Services;
using Admission.Domain.Entities;
using MailContracts;
using MassTransit;

namespace Admission.Messaging;

public class MassTransitAdmissionEventsPublisher(IPublishEndpoint publisher) : IMessagePublisherService
{
    public Task SendAdmissionStatusUpdatedMessage(string email, string name, AdmissionStatus status)
    {
        return publisher.Publish(new AdmissionStatusChangedMessage(new MailRecipient(email, name), status.ToString()));
    }

    public Task SendAdmissionAssignedToManagerMessage(
        string managerEmail, string managerName,
        string applicantEmail, string applicantName,
        Guid admissionId)
    {
        return publisher.Publish(new AdmissionAssignedToManagerMessage(
            new MailRecipient(managerEmail, managerName),
            new MailRecipient(applicantEmail, applicantName),
            admissionId));
    }

    public Task SendAdmissionAssignedToApplicantMessage(
        string applicantEmail, string applicantName,
        string managerEmail, string managerName,
        Guid admissionId)
    {
        return publisher.Publish(new AdmissionManagerAssignedToApplicantMessage(
            new MailRecipient(applicantEmail, applicantName),
            new MailRecipient(managerEmail, managerName),
            admissionId));
    }
}