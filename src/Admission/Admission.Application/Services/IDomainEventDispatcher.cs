namespace Admission.Application.Services;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IReadOnlyList<object> events, CancellationToken ct = default);
}
