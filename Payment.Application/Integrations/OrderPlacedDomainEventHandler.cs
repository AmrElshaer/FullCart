using Contracts.Events;
using MediatR;
using Payment.Application.Common;

namespace Payment.Application.Integrations;

public class OrderPlacedDomainEventHandler : INotificationHandler<OrderPlacedEvent>
{
    private readonly IPaymentDbContext _paymentDbContext;

    public OrderPlacedDomainEventHandler(IPaymentDbContext paymentDbContext)
    {
        _paymentDbContext = paymentDbContext;
    }

    public async Task Handle(OrderPlacedEvent notification, CancellationToken cancellationToken)
    {
        var newPayment = new Domain.Payments.Payment(notification.OrderId);

        await _paymentDbContext.Payments.AddAsync(newPayment, cancellationToken);
    }
}