using Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Common.Interceptors;

public class PublishDomainEventsInterceptor : SaveChangesInterceptor
{
    private readonly IMediator _publisher;

    public PublishDomainEventsInterceptor(IMediator publisher)
    {
        _publisher = publisher;
    }

    public override async ValueTask<int> SavedChangesAsync
    (
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default
    )
    {
        if (eventData.Context is not null)
        {
            await PublishDomainEventsAsync(eventData.Context, cancellationToken);
        }

        return result;
    }

    private async Task PublishDomainEventsAsync(DbContext context, CancellationToken cancellationToken)
    {
        var domainEventEntities = context.ChangeTracker.Entries<Entity>()
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

                await _publisher.Publish(domainEvent, cancellationToken);
            }
        }

        var tasks = integrationEvents
            .Select(async (domainEvent) =>
            {
                await _publisher.Publish(domainEvent, cancellationToken);
            });

        await Task.WhenAll(tasks);
    }

    private static INotification? CreateDomainEventNotification(IDomainEvent domainEvent)
    {
        var genericDispatcherType = typeof(IntegrationEvent<>).MakeGenericType(domainEvent.GetType());

        return (INotification?) Activator.CreateInstance(genericDispatcherType, domainEvent);
    }
}
