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

            var integrationEvents = new Queue<INotification>();

            foreach (var entity in domainEventEntities)
            {
                while (entity.DomainEvents.TryTake(out var domainEvent))
                {
                    var integrationEvent = CreateDomainEventNotification(domainEvent);

                    if (integrationEvent is not null)
                    {
                        var handlerType = typeof(INotificationHandler<>).MakeGenericType(integrationEvent.GetType());

                        var integrationEventHandler = _serviceProvider.GetService(handlerType);

                        if (integrationEventHandler is not null)
                        {
                            integrationEvents.Enqueue(integrationEvent);
                        }
                    }

                    await _publisher.Publish(domainEvent);
                }
            }

            while (integrationEvents.TryDequeue(out var integrationEvent))
            {
                await _publisher.Publish(integrationEvent);
            }
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
