using Application.Common.Interfaces.Data;
using Domain.Orders.Events;
using Domain.Payments;
using Microsoft.Extensions.Logging;

namespace Application.Orders.Commands.CreateOrder;

public class OrderPlacedDomainEventHandler(
    ICartDbContext cartDbContext,
    IHttpClientFactory httpClientFactory,
    ILogger<OrderPlacedDomainEventHandler> logger)
    : INotificationHandler<OrderPlacedEvent>
{
    public async Task Handle(OrderPlacedEvent notification, CancellationToken cancellationToken)
    {
        var newPayment = new Payment(notification.OrderId);

        await cartDbContext.Payments.AddAsync(newPayment, cancellationToken);
    }
}