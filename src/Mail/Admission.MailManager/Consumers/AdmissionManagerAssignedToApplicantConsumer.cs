using MailContracts;
using MassTransit;

namespace Admission.MailManager.Consumers;

public class AdmissionManagerAssignedToApplicantConsumer : IConsumer<AdmissionManagerAssignedToApplicantMessage>
{
    public Task Consume(ConsumeContext<AdmissionManagerAssignedToApplicantMessage> context) => Task.CompletedTask;
}
