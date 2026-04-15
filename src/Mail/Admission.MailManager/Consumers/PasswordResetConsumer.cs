using MailContracts;
using MassTransit;

namespace Admission.MailManager.Consumers;

public class PasswordResetConsumer : IConsumer<PasswordResetMessage>
{
    public Task Consume(ConsumeContext<PasswordResetMessage> context) => Task.CompletedTask;
}
