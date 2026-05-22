using Admission.Domain.Entities;

namespace Admission.Application.Applicants.DTOs;

public sealed record CreateApplicantProfileDto(
    string Name,
    DateTime BirthDate,
    Gender Gender,
    string PhoneNumber,
    string? Citizenship = null);
