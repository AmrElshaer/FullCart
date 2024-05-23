using Application.Common.Interfaces.Hubs;
using Domain.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Hubs.OrderHub;
[AllowAnonymous]
public class OrderStatusHub : Hub<IOrderStatusHub>
{
    public Task StartTrackingOrder(string orderId) =>
        Groups.AddToGroupAsync(
            Context.ConnectionId, orderId);
    // public Task StartTrackingOrder(Order order) =>
    //     Groups.AddToGroupAsync(
    //         Context.ConnectionId, order.ToOrderTrackingGroupId());

    public Task StopTrackingOrder(string orderId) =>
        Groups.RemoveFromGroupAsync(
            Context.ConnectionId, orderId);
}
