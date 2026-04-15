namespace Admission.Auth.Messaging;

public interface IMailEventPublisher
{
    Task PublishNewRegistrationAsync(string email, CancellationToken cancellationToken = default);
    Task PublishManagerCreatedAsync(string email, string roleName, string temporaryPassword, CancellationToken cancellationToken = default);
    Task PublishStaffPasswordResetAsync(string email, string roleName, string temporaryPassword, CancellationToken cancellationToken = default);
    Task PublishEmailConfirmationAsync(string email, string confirmationToken, CancellationToken cancellationToken = default);
    Task PublishPasswordResetAsync(string email, string resetToken, CancellationToken cancellationToken = default);
    Task PublishStaffInvitationAsync(string email, string roleName, string invitationToken, Guid? facultyId, CancellationToken cancellationToken = default);
}
