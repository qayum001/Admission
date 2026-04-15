namespace Admission.Auth.Messaging;

public sealed class MessagingOptions
{
    public const string SectionName = "MessagingParams";

    public string Host { get; set; } = string.Empty;
    public ushort Port { get; set; }
    public string User { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
