using Admission.Domain.Entities;

namespace Admission.Domain.Events;

public record NewAdmissionAddedDomainEvent(Applicant Applicant);