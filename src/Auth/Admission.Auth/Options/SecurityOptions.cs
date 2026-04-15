namespace Admission.Auth.Options;

public sealed class SecurityOptions
{
    public const string SectionName = "Security";

    public string TokenPepper { get; set; } = string.Empty;
    public int ActionTokenLifetimeMinutes { get; set; }
}
