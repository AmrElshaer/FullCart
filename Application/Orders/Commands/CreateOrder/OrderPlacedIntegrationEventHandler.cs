using Domain.Common;
using Domain.Orders.Events;
using DotNetCore.CAP;
using Microsoft.Extensions.Logging;

namespace Application.Orders.Commands.CreateOrder;

public class OrderPlacedIntegrationEventHandler : ICapSubscribe
{
    private readonly ILogger<OrderPlacedIntegrationEventHandler> _logger;

    public OrderPlacedIntegrationEventHandler(ILogger<OrderPlacedIntegrationEventHandler> logger)
    {
        _logger = logger;
    }

    [CapSubscribe(IntegrationEventConstants.OrderConstant.OrderPlaced)]
    public async Task HandleAsync(OrderPlacedIntegrationEvent notification)
    {
        _logger.LogInformation("order placed {@Notification}", notification);
        await Task.CompletedTask;
    }
}
