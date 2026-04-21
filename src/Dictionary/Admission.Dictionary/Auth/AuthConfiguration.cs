namespace Admission.Dictionary.Auth;

public class AuthConfiguration
{
    public string MetadataAddress { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string RoleClaimType { get; set; } = string.Empty;
}