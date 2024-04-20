using Domain.Common;
using Domain.Orders.Events;
using DotNetCore.CAP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Orders.Commands.CreateOrder;

public class OrderPlacedIntegrationEventHandler : INotificationHandler<OrderPlacedIntegrationEvent>
{
    private readonly ILogger<OrderPlacedIntegrationEventHandler> _logger;

    public OrderPlacedIntegrationEventHandler(ILogger<OrderPlacedIntegrationEventHandler> logger)
    {
        _logger = logger;
    }

   

    public async Task Handle(OrderPlacedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("order placed {@Notification}", notification);
        await Task.Delay(2000);
        await Task.CompletedTask;
    }
}
