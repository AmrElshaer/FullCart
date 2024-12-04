using Application.Common.Interfaces.Data;
using Domain.Orders.Events;
using Domain.Payments;

namespace Application.Orders.Commands.CreateOrder;

public class OrderPlacedDomainEventHandler : INotificationHandler<OrderPlacedEvent>
{
    private readonly IPaymentDbContext _paymentDbContext;

    public OrderPlacedDomainEventHandler(IPaymentDbContext paymentDbContext)
    {
        _paymentDbContext = paymentDbContext;
    }

    public async Task Handle(OrderPlacedEvent notification, CancellationToken cancellationToken)
    {
        var newPayment = new Payment(notification.OrderId);

        await _paymentDbContext.Payments.AddAsync(newPayment, cancellationToken);
    }
}