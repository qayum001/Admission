using Admission.Application.Services;
using Admission.Domain.Events;
using LiteBus.Events.Abstractions;

namespace Admission.Application.Applicants.Events;

public sealed class AdmissionStatusUpdateDomainEventHandler(IMessagePublisherService messagePublisher)
    : IEventHandler<AdmissionStatusUpdateDomainEvent>
{
    public async Task HandleAsync(AdmissionStatusUpdateDomainEvent message,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var applicant = message.Admission.Applicant;

        await messagePublisher.SendAdmissionStatusUpdatedMessage(applicant.Email.Value, applicant.Name.Value,
            message.Admission.AdmissionStatus);
    }
}
