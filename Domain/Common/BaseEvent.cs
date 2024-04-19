using MediatR;

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

    public DateTime OccurredOn { get; }

    string Type { get; }
}

public class IntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; } = Guid.NewGuid();

    public DateTime OccurredOn { get; }=DateTime.Now;

    public string Type { get; init; } = default!;
}
