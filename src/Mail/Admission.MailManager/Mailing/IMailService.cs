namespace Admission.MailManager.Mailing;

public interface IMailService
{
    public Task Send(string to, string name, string subject, string body);
}