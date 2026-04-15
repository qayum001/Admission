namespace Admission.Auth.Options;

public sealed class PasswordPolicyOptions
{
    public const string SectionName = "PasswordPolicy";

    public int MinLength { get; set; }
    public bool RequireUpper { get; set; }
    public bool RequireLower { get; set; }
    public bool RequireDigit { get; set; }
    public bool RequireNonAlphanumeric { get; set; }
    public List<string> DisallowedPasswords { get; set; } = [];
}
