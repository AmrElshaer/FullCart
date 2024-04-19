using Application.Common.Interfaces;
using Domain.Common;
using Domain.Outbox;
using Infrastructure.Common.Persistence;
using MediatR;
using Newtonsoft.Json;

namespace Infrastructure.Common
{
    internal class EventDispatcher : IEventDispatcher
    {
        private readonly IMediator _publisher;
        private readonly CartDbContext _cartDbContext;

        public EventDispatcher(IMediator publisher, CartDbContext cartDbContext)
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
            {
                while (entity.DomainEvents.TryTake(out var domainEvent))
                {
                    await _publisher.Publish(domainEvent);
                }
            }
            var integrationEventsEntities =_cartDbContext.ChangeTracker.Entries<Entity>()
                .Select(po => po.Entity)
                .Where(po => po.IntegrationEvents.Any())
                .ToArray();
            foreach (var entity in integrationEventsEntities)
            {
                while (entity.IntegrationEvents.TryTake(out var integrationEvent))
                {
                    var data = JsonConvert.SerializeObject(integrationEvent);
                    var outboxMessage = new OutboxMessage(
                        integrationEvent.OccurredOn,
                        data);
                    this._cartDbContext.OutboxMessages.Add(outboxMessage);
                }
               
              
            }
        }

        public async Task Dispatch()
        {
            await this.PublishDomainEventsAsync();
        }
    }
}
