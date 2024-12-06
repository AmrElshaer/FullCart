using BuildingBlocks.Domain.Common;
using Payment.Contracts.Events.DomainEvents;
using Payment.Contracts.Events.IntegrationEvents;

namespace Payment.Domain.Payments;

public class Payment : Entity
{
    public Guid OrderId { get; private set; }
    public PaymentStatus PaymentStatus { get; private set; }

    private Payment()
    {
    }

    public Payment(Guid orderId)
    {
        OrderId = orderId;
        PaymentStatus = PaymentStatus.ToPay;
        AddDomainEvent(new PaymentCreatedEvent() { OrderId = orderId, PaymentId = Id });
        AddIntegrationEvent(new PaymentCreatedNotification(Id));
    }
}