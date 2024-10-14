using Domain.Common;
using Domain.Orders;

namespace Domain.Payments.Events;

public class PaymentCreatedEvent : DomainEvent
{
    public Guid PaymentId { get; init; }

    public required OrderId OrderId { get; init; }
}