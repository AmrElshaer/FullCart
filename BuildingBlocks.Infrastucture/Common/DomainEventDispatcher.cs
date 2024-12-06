using Application.Common.Interfaces.Event;
using BuildingBlocks.Application.Common.Interfaces;
using MediatR;

namespace BuildingBlocks.Infrastucture.Common;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IDomainEventsAccessor _domainEventsAccessor;
    private readonly IMediator _publisher;

    public DomainEventDispatcher(IMediator publisher, IDomainEventsAccessor domainEventsAccessor)
    {
        _publisher = publisher;
        _domainEventsAccessor = domainEventsAccessor;
    }


    public async Task Dispatch()
    {
        var domainEvents = _domainEventsAccessor.GetAllDomainEvents();
        _domainEventsAccessor.ClearAllDomainEvents();

        foreach (var domainEvent in domainEvents) await _publisher.Publish(domainEvent);
    }
}