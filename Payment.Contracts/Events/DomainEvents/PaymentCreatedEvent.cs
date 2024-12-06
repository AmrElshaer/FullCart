using BuildingBlocks.Domain.Common;

namespace Payment.Contracts.Events.DomainEvents;

public class PaymentCreatedEvent : DomainEvent
{
    public Guid PaymentId { get; init; }

    public required Guid OrderId { get; init; }
}