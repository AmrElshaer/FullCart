using Domain.Common;
using Domain.Payments.Events;
using Domain.Orders;

namespace Domain.Payments;

public class Payment : Entity
{
    public OrderId OrderId { get; private set; }
    public PaymentStatus PaymentStatus { get; private set; }

    private Payment()
    {
    }

    public Payment(OrderId orderId)
    {
        OrderId = orderId;
        PaymentStatus = PaymentStatus.ToPay;
        AddDomainEvent(new PaymentCreatedEvent() { OrderId = orderId, PaymentId = Id });
        AddIntegrationEvent(new PaymentCreatedNotification(Id));
    }
}