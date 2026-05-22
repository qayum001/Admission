using LiteBus.Events.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Admission.Application.Services;

public sealed class LiteBusDomainEventDispatcher(IServiceProvider serviceProvider) : IDomainEventDispatcher
{
    public async Task DispatchAsync(IReadOnlyList<object> events, CancellationToken ct = default)
    {
        foreach (var @event in events)
        {
            var handlerInterfaceType = typeof(IEventHandler<>).MakeGenericType(@event.GetType());
            var handlers = serviceProvider.GetServices(handlerInterfaceType);

            foreach (var handler in handlers)
            {
                if (handler is null) continue;
                await ((dynamic)handler).HandleAsync((dynamic)@event, ct);
            }
        }
    }
}
