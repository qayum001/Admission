namespace Admission.Auth.Messaging;

public interface IMailEventPublisher
{
    Task PublishManagerCreatedAsync(string email, string roleName, string temporaryPassword, CancellationToken cancellationToken = default);
    Task PublishEmailConfirmationAsync(string email, string confirmationToken, CancellationToken cancellationToken = default);
    Task PublishPasswordResetAsync(string email, string resetToken, CancellationToken cancellationToken = default);
}
