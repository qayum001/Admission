namespace Admission.MailManager.Templating;

public interface IEmailTemplateService
{
    string Render<TMessage>(object model);
    string Render(Type messageType, object model);
}
