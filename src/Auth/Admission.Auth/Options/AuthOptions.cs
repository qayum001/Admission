namespace Admission.Auth.Options;

public sealed class AuthOptions
{
    public const string SectionName = "Auth";

    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenMinutes { get; set; }
    public int RefreshTokenDays { get; set; }
    public int ClockSkewSeconds { get; set; }
    public int RefreshTokenLength { get; set; }
}
