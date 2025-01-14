﻿using Application.Common.Interfaces.Data;
using Domain.Orders.Events;
using Domain.Payments;

namespace Application.Orders.Commands.CreateOrder;

public class OrderPlacedDomainEventHandler : INotificationHandler<OrderPlacedEvent>
{
    private readonly ICartDbContext _cartDbContext;

    public OrderPlacedDomainEventHandler(ICartDbContext cartDbContext)
    {
        _cartDbContext = cartDbContext;
    }

    public async Task Handle(OrderPlacedEvent notification, CancellationToken cancellationToken)
    {
        var newPayment = new Payment(notification.OrderId);

        await _cartDbContext.Payments.AddAsync(newPayment, cancellationToken);
    }
}