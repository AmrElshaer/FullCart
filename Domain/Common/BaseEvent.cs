using MediatR;
using System.Text.Json.Serialization;

namespace Domain.Common;


public interface IDomainEvent : INotification
{
    bool IsPublished { get; set; }
}

public abstract class DomainEvent : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();

    public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;

    public bool IsPublished { get; set; }
}
public interface IIntegrationEvent<out TEventType> : IIntegrationEvent
{
    TEventType DomainEvent { get; }
}
public interface IIntegrationEvent : INotification
{
    Guid Id { get; }
}

public class IntegrationEvent<T> : IIntegrationEvent<T> where T : IDomainEvent
{
    [JsonIgnore]
    public T DomainEvent { get; }

    public Guid Id { get;  }

    public IntegrationEvent(T domainEvent)
    {
        this.Id = Guid.NewGuid();
        this.DomainEvent = domainEvent;
    }
}