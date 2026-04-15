using MailContracts;
using MassTransit;

namespace Admission.MailManager.Consumers;

public class StaffInvitationConsumer : IConsumer<StaffInvitationMessage>
{
    public Task Consume(ConsumeContext<StaffInvitationMessage> context) => Task.CompletedTask;
}
