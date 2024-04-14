using Application.Common.Interfaces;
using Domain.Common;
using Infrastructure.Common.Persistence;
using MediatR;

namespace Infrastructure.Common
{
    internal class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IMediator _publisher;
        private readonly CartDbContext _cartDbContext;
        private readonly IServiceProvider _serviceProvider;

        public DomainEventDispatcher(IMediator publisher, CartDbContext cartDbContext, IServiceProvider serviceProvider)
        {
            _publisher = publisher;
            _cartDbContext = cartDbContext;
            _serviceProvider = serviceProvider;
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

            var integrationEventsEntities=_cartDbContext.ChangeTracker.Entries<Entity>()
                .Select(po => po.Entity)
                .Where(po => po.IntegrationEvents.Any())
                .ToArray();

            foreach (var entity in integrationEventsEntities)
            {
                while (entity.IntegrationEvents.TryTake(out var integrationEvent))
                {
                    await _publisher.Publish(integrationEvent);
                }
            }
        }


        public async Task Dispatch()
        {
            await this.PublishDomainEventsAsync();
        }
    }
}
