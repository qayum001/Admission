using MailContracts;
using MassTransit;

namespace Admission.MailManager.Consumers;

public class EmailConfirmationConsumer : IConsumer<EmailConfirmationMessage>
{
    public Task Consume(ConsumeContext<EmailConfirmationMessage> context) => Task.CompletedTask;
}
