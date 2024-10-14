using Domain.Common;

namespace Domain.Orders.Events;

public class OrderPlacedIntegrationEvent : IntegrationEvent
{
    public OrderId OrderId { get; }

    public OrderPlacedIntegrationEvent(OrderId orderId)
    {
        OrderId = orderId;
        Type = IntegrationEventConstants.OrderConstant.OrderPlaced;
    }
}