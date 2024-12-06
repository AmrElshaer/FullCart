using Application.Common.Interfaces.Hubs;
using BuildingBlocks.Domain.Common;
using Domain.Orders.Events;
using DotNetCore.CAP;
using Microsoft.Extensions.Logging;

namespace Application.Orders.Commands.ChangeOrderStatus;

public class OrderStatusChangeIntegrationEventHandler(
    ILogger<OrderStatusChangeIntegrationEventHandler> logger,
    IOrderHub orderHub) : ICapSubscribe
{
    [CapSubscribe(IntegrationEventConstants.OrderConstant.OrderStatusChanged)]
    public async Task HandleAsync(OrderStatusChangeIntegrationEvent notification)
    {
        logger.LogInformation("order status change {@Notification}", notification);
        orderHub.SendOrderStatusChanged(notification.OrderId, notification.OrderStatus);
        await Task.CompletedTask;
    }
}