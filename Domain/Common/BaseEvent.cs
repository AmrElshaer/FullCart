using MediatR;

namespace Domain.Common;


public abstract class DomainEvent : INotification
{
    public Guid Id { get; } = Guid.NewGuid();

    public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;

    public bool IsPublished { get; set; }
}

public class IntegrationEvent : INotification
{
    public Guid Id { get; } = Guid.NewGuid();

    public DateTime OccurredOn { get; }=DateTime.Now;
}
