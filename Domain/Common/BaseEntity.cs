using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Common;

public abstract class BaseEntity<TId>: BaseEntity,IComparable, IComparable<BaseEntity<TId>>
    where TId : IComparable<TId>
{
    
    public  TId Id { get; protected set; } = default!;
    
    protected BaseEntity()
    {
    }

    protected BaseEntity(TId id)
    {
        Id = id;
    }
    public override bool Equals(object? obj)
    {
        if (obj is not BaseEntity<TId> other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (IsTransient() || other.IsTransient())
            return false;

        return Id.Equals(other.Id);
    }

    private bool IsTransient()
    {
        return Id.Equals(default(TId));
    }

    public static bool operator ==(BaseEntity<TId>? a, BaseEntity<TId>? b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(BaseEntity<TId> a, BaseEntity<TId> b)
    {
        return !(a == b);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode() ^ GetType().GetHashCode();
    }

    public virtual int CompareTo(BaseEntity<TId>? other)
    {
        if (other is null)
            return 1;

        if (ReferenceEquals(this, other))
            return 0;

        return Id.CompareTo(other.Id);
    }

    public virtual int CompareTo(object? other)
    {
        return CompareTo(other as BaseEntity<TId>);
    }
}
public abstract class BaseEntity
{

    private readonly ConcurrentQueue<DomainEvent> _domainEvents = new();
    private readonly ConcurrentQueue<IntegrationEvent> _integrationEvents = new();

    [NotMapped]
    public IProducerConsumerCollection<DomainEvent> DomainEvents => _domainEvents;
    [NotMapped]
    public IProducerConsumerCollection<IntegrationEvent> IntegrationEvents => _integrationEvents;
    protected BaseEntity()
    {
    }
    protected void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Enqueue(@domainEvent);
    }
    protected void AddIntegrationEvent(IntegrationEvent integrationEvent)
    {
        _integrationEvents.Enqueue(integrationEvent);
    }
    
}

public abstract class Entity : BaseEntity<Guid>
{
    
}