using Application.Common.Interfaces;
using Domain.Orders.Events;
using Domain.Payments;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Application.Orders.Commands.CreateOrder
{
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

            await this._cartDbContext.Payments.AddAsync(newPayment,cancellationToken);
        }
    }
    public class OrderPlacedIntegrationEventHandler : INotificationHandler<IntegrationEvent<OrderPlacedEvent>>
    {
        private readonly ICartDbContext _cartDbContext;

        public OrderPlacedIntegrationEventHandler(ICartDbContext cartDbContext)
        {
            _cartDbContext = cartDbContext;
        }


        public async Task Handle(IntegrationEvent<OrderPlacedEvent> notification, CancellationToken cancellationToken)
        {
           await Task.CompletedTask;
        }
    }
}
