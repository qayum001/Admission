namespace Admission.Auth.Options;

public sealed class SigningKeyOptions
{
    public const string SectionName = "SigningKeys";

    public int RotateAfterDays { get; set; } = 30;
    public int RetainDays { get; set; } = 90;
    public int RsaKeySize { get; set; } = 2048;
}
