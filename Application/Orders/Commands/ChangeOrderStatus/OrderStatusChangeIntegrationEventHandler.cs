using Application.Common.Interfaces.Hubs;
using Application.Orders.Commands.CreateOrder;
using Domain.Common;
using Domain.Orders.Events;
using DotNetCore.CAP;
using Microsoft.Extensions.Logging;

namespace Application.Orders.Commands.ChangeOrderStatus;

public class OrderStatusChangeIntegrationEventHandler: ICapSubscribe
{
    private readonly ILogger<OrderStatusChangeIntegrationEventHandler> _logger;
    private readonly IOrderHub _orderHub;

    public OrderStatusChangeIntegrationEventHandler(ILogger<OrderStatusChangeIntegrationEventHandler> logger,IOrderHub orderHub)

    {
        _logger = logger;
        _orderHub = orderHub;
    }

    [CapSubscribe(IntegrationEventConstants.OrderConstant.OrderStatusChanged)]
    public async Task HandleAsync(OrderStatusChangeIntegrationEvent notification)
    {
        _logger.LogInformation("order status change {@Notification}", notification);
        _orderHub.SendOrderStatusChanged(notification.OrderId,notification.OrderStatus);
        await Task.CompletedTask;
    }
}