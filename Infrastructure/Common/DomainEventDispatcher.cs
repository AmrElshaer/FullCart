using Application.Common.Interfaces;
using Application.Common.Interfaces.Event;
using Domain.Common;
using Infrastructure.Common.Persistence;
using MediatR;

namespace Infrastructure.Common;

internal class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IMediator _publisher;
    private readonly IDomainEventsAccessor _domainEventsAccessor;

    public DomainEventDispatcher(IMediator publisher, IDomainEventsAccessor domainEventsAccessor)
    {
        _publisher = publisher;
        _domainEventsAccessor = domainEventsAccessor;
    }


    public async Task Dispatch()
    {
         var domainEvents = _domainEventsAccessor.GetAllDomainEvents();
         _domainEventsAccessor.ClearAllDomainEvents();

         foreach (var domainEvent in domainEvents)
         {
             await _publisher.Publish(domainEvent);
         }
         
    }
}