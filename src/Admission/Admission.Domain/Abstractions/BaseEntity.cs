namespace Admission.Domain.Abstractions;

public abstract class BaseEntity : IEntity, IEquatable<BaseEntity>
{
    private readonly List<object> _events = [];

    protected BaseEntity()
    {
    }

    protected BaseEntity(Guid guid)
    {
        Id = guid;
    }

    public Guid Id { get; private set; }
    public IReadOnlyList<object> Events => _events.AsReadOnly();

    protected void AddEvent(object @event)
    {
        _events.Add(@event);
    }

    public void ClearEvents() => _events.Clear();
    
    public override int GetHashCode()
    {
        return HashCode.Combine(_events, Id);
    }

    public bool Equals(BaseEntity? other)
    {
        return other is not null && Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        return obj is BaseEntity other && Id.Equals(other.Id);
    }
}
