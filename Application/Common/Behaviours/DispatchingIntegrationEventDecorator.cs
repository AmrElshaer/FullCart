using Application.Common.Interfaces;
using MediatR;

namespace Application.Common.Behaviours;

public class DispatchingIntegrationEventDecorator<TNotification> : INotificationHandler
    <TNotification> where TNotification : INotification
{
    private readonly INotificationHandler<TNotification> _inner;
    private readonly IEventDispatcher _eventDispatcher;

    public DispatchingIntegrationEventDecorator(INotificationHandler<TNotification> inner, IEventDispatcher eventDispatcher)
    {
        _inner = inner;
        _eventDispatcher = eventDispatcher;
    }

    public async Task Handle(TNotification notification, CancellationToken cancellationToken)
    {
        await _inner.Handle(notification, cancellationToken);
        await _eventDispatcher.Dispatch();
    }
}
