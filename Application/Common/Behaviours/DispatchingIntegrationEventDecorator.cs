using Application.Common.Interfaces;
using Application.Common.Interfaces.Event;
using MediatR;

namespace Application.Common.Behaviours;

public class DispatchingIntegrationEventDecorator<TNotification> : INotificationHandler
    <TNotification> where TNotification : INotification
{
    private readonly INotificationHandler<TNotification> _inner;
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public DispatchingIntegrationEventDecorator(INotificationHandler<TNotification> inner,
        IDomainEventDispatcher domainEventDispatcher)
    {
        _inner = inner;
        _domainEventDispatcher = domainEventDispatcher;
    }

    public async Task Handle(TNotification notification, CancellationToken cancellationToken)
    {
        await _inner.Handle(notification, cancellationToken);
        await _domainEventDispatcher.Dispatch();
    }
}