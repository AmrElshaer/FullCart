using Domain.Common;

namespace Domain.Orders.Events;

public class OrderStatusChangeIntegrationEvent : IntegrationEvent
{
    public OrderId OrderId { get; }

    public OrderStatus OrderStatus { get; }

    public OrderStatusChangeIntegrationEvent(OrderId orderId, OrderStatus orderStatus)
    {
        OrderId = orderId;
        OrderStatus = orderStatus;
        Type = IntegrationEventConstants.OrderConstant.OrderStatusChanged;
    }
}