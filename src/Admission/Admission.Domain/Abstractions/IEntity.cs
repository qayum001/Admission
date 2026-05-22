namespace Admission.Domain.Abstractions;

public interface IEntity
{
    Guid Id { get; }
    IReadOnlyList<object> Events { get; }
}