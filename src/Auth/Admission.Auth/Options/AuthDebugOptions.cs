namespace Admission.Auth.Options;

public sealed class AuthDebugOptions
{
    public const string SectionName = "AuthDebug";

    public bool ExposeActionTokens { get; set; }
}
