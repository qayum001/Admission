using Admission.Application.DTO;
using Admission.Domain.Entities;

namespace Admission.Application.Applicants.DTOs;

public sealed class PassportDto
{
    public Guid Id { get; init; }
    public string SerialNumber { get; init; } = string.Empty;
    public DateTime GivenDate { get; init; }
    public string GivenBy { get; init; } = string.Empty;
    public string? ScanUrl { get; init; }
    public DateTime? ScanUrlExpiresAt { get; init; }

    public static PassportDto From(Passport passport, PresignedUrlResult? scanUrl) => new()
    {
        Id = passport.Id,
        SerialNumber = passport.SerialNumber.Value,
        GivenDate = passport.GivenDate,
        GivenBy = passport.GivenBy.Value,
        ScanUrl = scanUrl?.Url,
        ScanUrlExpiresAt = scanUrl?.ExpiresAt
    };
}
