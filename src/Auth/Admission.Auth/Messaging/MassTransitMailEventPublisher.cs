using MailContracts;
using MassTransit;

namespace Admission.Auth.Messaging;

public sealed class MassTransitMailEventPublisher(IPublishEndpoint publishEndpoint) : IMailEventPublisher
{
    public Task PublishNewRegistrationAsync(string email, CancellationToken cancellationToken = default)
    {
        return publishEndpoint.Publish(
            new NewRegistrationMessage(ToRecipient(email)),
            cancellationToken);
    }

    public Task PublishManagerCreatedAsync(string email, string roleName, string temporaryPassword, CancellationToken cancellationToken = default)
    {
        return publishEndpoint.Publish(
            new ManagerCreatedMessage(ToRecipient(email), roleName, temporaryPassword),
            cancellationToken);
    }

    public Task PublishStaffPasswordResetAsync(string email, string roleName, string temporaryPassword, CancellationToken cancellationToken = default)
    {
        return publishEndpoint.Publish(
            new StaffPasswordResetMessage(ToRecipient(email), roleName, temporaryPassword),
            cancellationToken);
    }

    public Task PublishEmailConfirmationAsync(string email, string confirmationToken, CancellationToken cancellationToken = default)
    {
        return publishEndpoint.Publish(
            new EmailConfirmationMessage(ToRecipient(email), confirmationToken),
            cancellationToken);
    }

    public Task PublishPasswordResetAsync(string email, string resetToken, CancellationToken cancellationToken = default)
    {
        return publishEndpoint.Publish(
            new PasswordResetMessage(ToRecipient(email), resetToken),
            cancellationToken);
    }

    public Task PublishStaffInvitationAsync(string email, string roleName, string invitationToken, Guid? facultyId, CancellationToken cancellationToken = default)
    {
        return publishEndpoint.Publish(
            new StaffInvitationMessage(ToRecipient(email), roleName, invitationToken, facultyId),
            cancellationToken);
    }

    private static MailRecipient ToRecipient(string email)
    {
        var atIndex = email.IndexOf('@');
        var name = atIndex > 0 ? email[..atIndex] : email;
        return new MailRecipient(email, name);
    }
}
