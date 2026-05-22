using Admission.Domain.Entities;

namespace Admission.Domain.Events;

public record PassportConfirmedDomainEvent(Applicant Applicant);