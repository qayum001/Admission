using MailContracts;
using MassTransit;

namespace Admission.MailManager.Consumers;

public class ManagerCreatedConsumer : IConsumer<ManagerCreatedMessage>
{
    public Task Consume(ConsumeContext<ManagerCreatedMessage> context) => Task.CompletedTask;
}
