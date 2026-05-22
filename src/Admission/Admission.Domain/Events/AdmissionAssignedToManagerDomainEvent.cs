using Admission.Domain.Entities;

namespace Admission.Domain.Events;

public sealed record AdmissionAssignedToManagerDomainEvent(Manager Manager, Entities.Admission Admission);