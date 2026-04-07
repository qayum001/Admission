using MailContracts;
using MassTransit;

namespace Admission.MailManager.Consumers;

public class AdmissionAssignedToManagerConsumer : IConsumer<AdmissionAssignedToManagerMessage>
{
    public Task Consume(ConsumeContext<AdmissionAssignedToManagerMessage> context) => Task.CompletedTask;
}
