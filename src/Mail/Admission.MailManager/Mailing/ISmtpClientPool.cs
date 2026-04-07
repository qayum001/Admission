namespace Admission.MailManager.Mailing;

public interface ISmtpClientPool
{
    ValueTask<SmtpClientLease> RentAsync(CancellationToken cancellationToken = default);
}
