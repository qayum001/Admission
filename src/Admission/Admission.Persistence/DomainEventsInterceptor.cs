using Admission.Application.Services;
using Admission.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Admission.Persistence;

public sealed class DomainEventsInterceptor(IDomainEventDispatcher dispatcher) : SaveChangesInterceptor
{
    private List<object> _pendingEvents = [];
    
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        CollectEvents(eventData.Context);
        DispatchEventsAsync(CancellationToken.None).GetAwaiter().GetResult();
        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        CollectEvents(eventData.Context);
        await DispatchEventsAsync(cancellationToken);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void CollectEvents(DbContext? context)
    {
        if (context is null) return;

        var entities = context.ChangeTracker
            .Entries<BaseEntity>()
            .Select(e => e.Entity)
            .Where(e => e.Events.Count > 0)
            .ToList();

        _pendingEvents = entities.SelectMany(e => e.Events).ToList();

        foreach (var entity in entities)
            entity.ClearEvents();
    }

    private async Task DispatchEventsAsync(CancellationToken ct)
    {
        if (_pendingEvents.Count == 0) return;

        var events = _pendingEvents;
        _pendingEvents = [];

        await dispatcher.DispatchAsync(events, ct);
    }
}
