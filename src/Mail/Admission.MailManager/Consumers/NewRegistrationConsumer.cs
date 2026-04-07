using MailContracts;
using MassTransit;

namespace Admission.MailManager.Consumers;

public class NewRegistrationConsumer : IConsumer<NewRegistrationMessage>
{
    public Task Consume(ConsumeContext<NewRegistrationMessage> context) => Task.CompletedTask;
}
