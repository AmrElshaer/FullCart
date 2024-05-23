using Domain.Common;

namespace Domain.Orders.Events;

public class OrderStatusChangeIntegrationEvent:IntegrationEvent
{
    public Guid OrderId { get; }

    public OrderStatus OrderStatus { get; }
    public OrderStatusChangeIntegrationEvent(Guid orderId,OrderStatus orderStatus)
    {
        OrderId = orderId;
        OrderStatus = orderStatus;
        Type = IntegrationEventConstants.OrderConstant.OrderStatusChanged;
    }
}
