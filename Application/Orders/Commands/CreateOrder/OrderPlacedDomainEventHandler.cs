using Application.Common.Interfaces;
using Domain.Orders.Events;
using Domain.Payments;
using MediatR;

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

            await this._cartDbContext.Payments.AddAsync(newPayment, cancellationToken);
        }
    }

    public class OrderPlacedIntegrationEventHandler : INotificationHandler<OrderPlacedIntegrationEvent>
    {
        private readonly ICartDbContext _cartDbContext;

        public OrderPlacedIntegrationEventHandler(ICartDbContext cartDbContext)
        {
            _cartDbContext = cartDbContext;
        }

        public async Task Handle(OrderPlacedIntegrationEvent notification, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
    }
}
