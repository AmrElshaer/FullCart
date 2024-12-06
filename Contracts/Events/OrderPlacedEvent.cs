using BuildingBlocks.Domain.Common;

namespace Contracts.Events;

public class OrderPlacedEvent : DomainEvent
{
    public required Guid OrderId { get; init; }

    public Guid CustomerId { get; init; }
}