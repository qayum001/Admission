using Admission.Application.Services;
using Admission.Domain.Events;
using LiteBus.Events.Abstractions;

namespace Admission.Application.Managers.Events;

public sealed class AdmissionAssignedToManagerDomainEventHandler(IMessagePublisherService messagePublisher)
    : IEventHandler<AdmissionAssignedToManagerDomainEvent>
{
    public async Task HandleAsync(AdmissionAssignedToManagerDomainEvent message,
        CancellationToken cancellationToken = default)
    {
        var manager = message.Manager;
        var admission = message.Admission;
        var applicant = admission.Applicant;

        await messagePublisher.SendAdmissionAssignedToManagerMessage(
            manager.Email,
            manager.Name.Value,
            applicant.Email.Value,
            applicant.Name.Value,
            admission.Id);

        await messagePublisher.SendAdmissionAssignedToApplicantMessage(
            applicant.Email.Value,
            applicant.Name.Value,
            manager.Email,
            manager.Name.Value,
            admission.Id);
    }
}
