using Application.Common.Interfaces.Hubs;
using Domain.Orders;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Hubs.OrderHub;

public class OrderHub : IOrderHub
{
    private readonly IHubContext<OrderStatusHub, IOrderStatusHub> _orderStatusHub;

    public OrderHub(IHubContext<OrderStatusHub, IOrderStatusHub> orderStatusHub)
    {
        _orderStatusHub = orderStatusHub;
    }

    public void SendOrderStatusChanged(OrderId orderId, OrderStatus orderStatus)
    {
        _orderStatusHub.Clients.Groups(orderId.Value.ToString()).OrderStatusChanged(orderStatus);
    }
}