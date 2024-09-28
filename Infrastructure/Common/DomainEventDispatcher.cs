using Application.Common.Interfaces;
using Application.Common.Interfaces.Event;
using Domain.Common;
using Infrastructure.Common.Persistence;
using MediatR;

namespace Infrastructure.Common;

internal class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IMediator _publisher;
    private readonly CartDbContext _cartDbContext;

    public DomainEventDispatcher(IMediator publisher, CartDbContext cartDbContext)
    {
        _publisher = publisher;
        _cartDbContext = cartDbContext;
    }

    private async Task PublishDomainEventsAsync()
    {
        var domainEventEntities = _cartDbContext.ChangeTracker.Entries<Entity>()
            .Select(po => po.Entity)
            .Where(po => po.DomainEvents.Any())
            .ToArray();

        foreach (var entity in domainEventEntities)
            while (entity.DomainEvents.TryTake(out var domainEvent))
                await _publisher.Publish(domainEvent);
    }

    public async Task Dispatch()
    {
        await PublishDomainEventsAsync();
    }
}