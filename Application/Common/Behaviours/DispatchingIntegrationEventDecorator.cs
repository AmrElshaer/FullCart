using Application.Common.Interfaces;
using MediatR;

namespace Application.Common.Behaviours;

public class DispatchingIntegrationEventDecorator<TNotification> : INotificationHandler
    <TNotification> where TNotification : INotification
{
    private readonly INotificationHandler<TNotification> _inner;
    private readonly ICartDbContext _cartDbContext;

    public DispatchingIntegrationEventDecorator(INotificationHandler<TNotification> inner, ICartDbContext cartDbContext)
    {
        _inner = inner;
        _cartDbContext = cartDbContext;
    }

    public async Task Handle(TNotification notification, CancellationToken cancellationToken)
    {
        await _inner.Handle(notification, cancellationToken);
        await _cartDbContext.DispatchEvents(cancellationToken);
    }
}
