using MailContracts;
using MassTransit;

namespace Admission.MailManager.Consumers;

public class StaffPasswordResetConsumer : IConsumer<StaffPasswordResetMessage>
{
    public Task Consume(ConsumeContext<StaffPasswordResetMessage> context) => Task.CompletedTask;
}
