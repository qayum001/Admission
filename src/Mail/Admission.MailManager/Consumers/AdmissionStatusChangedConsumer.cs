using MailContracts;
using MassTransit;

namespace Admission.MailManager.Consumers;

public class AdmissionStatusChangedConsumer : IConsumer<AdmissionStatusChangedMessage>
{
    public Task Consume(ConsumeContext<AdmissionStatusChangedMessage> context) => Task.CompletedTask;
}
