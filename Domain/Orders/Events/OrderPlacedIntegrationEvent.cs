using Domain.Common;

namespace Domain.Orders.Events;

public class OrderPlacedIntegrationEvent : IntegrationEvent
{
    public Guid OrderId { get; }

    public OrderPlacedIntegrationEvent(Guid orderId)
    {
        OrderId = orderId;
    }
}
