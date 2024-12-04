using Application.Common.Interfaces.Event;

namespace Application.Common.Behaviours;

public class DomainEventsDispatcherDecorator<TNotification> : INotificationHandler
    <TNotification> where TNotification : INotification
{
    private readonly INotificationHandler<TNotification> _inner;
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public DomainEventsDispatcherDecorator(INotificationHandler<TNotification> inner,
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