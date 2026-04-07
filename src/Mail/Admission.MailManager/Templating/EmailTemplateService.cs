using System.Collections.Concurrent;
using Scriban;

namespace Admission.MailManager.Templating;

public class EmailTemplateService(IHostEnvironment hostEnvironment) : IEmailTemplateService
{
    private const string TemplatesDirectoryName = "Templating";
    private const string NestedTemplatesDirectoryName = "Templates";
    private const string TemplateFileExtension = ".html";

    private readonly string _templatesRootPath = Path.Combine(
        hostEnvironment.ContentRootPath,
        TemplatesDirectoryName,
        NestedTemplatesDirectoryName);
    private readonly ConcurrentDictionary<Type, Lazy<Template>> _templates = new();

    public string Render<TMessage>(object model)
    {
        return Render(typeof(TMessage), model);
    }

    public string Render(Type messageType, object model)
    {
        ArgumentNullException.ThrowIfNull(messageType);
        ArgumentNullException.ThrowIfNull(model);

        var template = GetOrLoadTemplate(messageType);

        return template.Render(model, member => member.Name);
    }

    private Template GetOrLoadTemplate(Type messageType)
    {
        var lazyTemplate = _templates.GetOrAdd(
            messageType,
            static (type, service) => new Lazy<Template>(
                () => service.LoadTemplate(type),
                LazyThreadSafetyMode.ExecutionAndPublication),
            this);

        try
        {
            return lazyTemplate.Value;
        }
        catch
        {
            _templates.TryRemove(messageType, out _);
            throw;
        }
    }

    private Template LoadTemplate(Type messageType)
    {
        var templatePath = Path.Combine(_templatesRootPath, $"{messageType.Name}{TemplateFileExtension}");

        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException(
                $"Email template for message type '{messageType.FullName}' was not found.",
                templatePath);
        }

        var templateText = File.ReadAllText(templatePath);
        var template = Template.Parse(templateText, templatePath);

        if (!template.HasErrors) return template;
        
        var errors = string.Join(Environment.NewLine, template.Messages.Select(static message => message.ToString()));
        throw new InvalidOperationException(
            $"Failed to parse email template '{templatePath}' for message type '{messageType.FullName}'.{Environment.NewLine}{errors}");
    }
}
