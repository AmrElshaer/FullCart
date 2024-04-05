using Application.Common.Interfaces;
using Domain.Common;
using Infrastructure.Common.Persistence;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Infrastructure.Common
{
    internal class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IPublisher _publisher;
        private readonly CartDbContext _cartDbContext;
        private readonly IServiceProvider _serviceProvider;

        public DomainEventDispatcher(IPublisher publisher,CartDbContext cartDbContext,IServiceProvider serviceProvider)
        {
           this._publisher = publisher;
            this._cartDbContext = cartDbContext;
            this._serviceProvider = serviceProvider;
        }

        public async Task Dispatch()
        {
            await PublishDomainEvents();
        }

        private async Task PublishDomainEvents()
        {
       
            var domainEntities = this._cartDbContext.ChangeTracker
                .Entries<Entity>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any()).ToList();

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            var domainEventNotifications = new List<IIntegrationEvent<IDomainEvent>>();
            foreach (var domainEvent in domainEvents)
            {
                Type domainEvenNotificationType = typeof(IIntegrationEvent<>);
                var domainNotificationWithGenericType = domainEvenNotificationType.MakeGenericType(domainEvent.GetType());
                var domainNotification = ActivatorUtilities.CreateInstance(_serviceProvider, domainNotificationWithGenericType, domainEvent) as IIntegrationEvent<IDomainEvent>;

                if (domainNotification != null)
                {
                    
                    domainEventNotifications.Add(domainNotification);
                }
            }

            domainEntities
                .ForEach(entity => entity.Entity.ClearDomainEvents());

            var tasks = domainEvents
                .Select(async (domainEvent) =>
                {
                    await _publisher.Publish(domainEvent);
                });

            await Task.WhenAll(tasks);
        }
    }
}
