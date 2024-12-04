using Application.Common.Interfaces;
using Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Common;

public class DomainEventsAccessor:IDomainEventsAccessor
{
    private readonly DbContext _dbContext;

    public DomainEventsAccessor(DbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public IReadOnlyCollection<DomainEvent> GetAllDomainEvents()
    {
        var domainEventEntities = this._dbContext.ChangeTracker.Entries<BaseEntity>()
            .Select(po => po.Entity)
            .Where(po => po.DomainEvents.Any())
            .ToArray();
        return domainEventEntities.SelectMany(po => po.DomainEvents).ToList();
    }

    public void ClearAllDomainEvents()
    {
        var domainEventEntities = this._dbContext.ChangeTracker.Entries<BaseEntity>()
            .Select(po => po.Entity)
            .Where(po => po.DomainEvents.Any())
            .ToList();
        domainEventEntities
            .ForEach(entity => entity.ClearDomainEvents());
    }
}