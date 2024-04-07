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

        public DomainEventDispatcher(IMediator publisher, CartDbContext cartDbContext)
        {
            this._publisher = publisher;
            _cartDbContext = cartDbContext;
        }

        private async Task PublishDomainEventsAsync()
        {
            var domainEventEntities = _cartDbContext.ChangeTracker.Entries<Entity>()
                .Select(po => po.Entity)
                .Where(po => po.DomainEvents.Any())
                .ToArray();

            var integrationEvents = new List<INotification>();

            foreach (var entity in domainEventEntities)
            {
                while (entity.DomainEvents.TryTake(out var domainEvent))
                {
                    var integrationEvent = CreateDomainEventNotification(domainEvent);

                    if (integrationEvent is not null)
                    {
                        integrationEvents.Add(integrationEvent);
                    }

                    await _publisher.Publish(domainEvent);
                }
            }

            var tasks = integrationEvents
                .Select(async (domainEvent) =>
                {
                    await _publisher.Publish(domainEvent);
                });

            await Task.WhenAll(tasks);
        }

        private static INotification? CreateDomainEventNotification(IDomainEvent domainEvent)
        {
            var genericDispatcherType = typeof(IntegrationEvent<>).MakeGenericType(domainEvent.GetType());

            return (INotification?) Activator.CreateInstance(genericDispatcherType, domainEvent);
        }

        public async Task Dispatch()
        {
            await this.PublishDomainEventsAsync();
        }
    }
}
