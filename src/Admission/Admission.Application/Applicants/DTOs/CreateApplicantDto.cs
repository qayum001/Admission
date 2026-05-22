using Admission.Domain.Entities;

namespace Admission.Application.Applicants.DTOs;

public record CreateApplicantDto(string Name, DateTime BirthDate, Gender Gender, string Email, string PhoneNumber);