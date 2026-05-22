namespace Admission.Api.Options;

public sealed class AdmissionProgramsOptions
{
    public const string SectionName = "AdmissionPrograms";

    public int MaxSelectedPrograms { get; init; } = 5;
}
