using Application.Common.Interfaces;
using Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Common;

public class IntegrationEventsAccessor:IIntegrationEventsAccessor
{
    private readonly DbContext _dbContext;

    public IntegrationEventsAccessor(DbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public IReadOnlyCollection<IntegrationEvent> GetIntegrationEvents()
    {
        var integrationEventsEntities = this._dbContext.ChangeTracker.Entries<BaseEntity>()
            .Select(po => po.Entity)
            .Where(po => po.IntegrationEvents.Any())
            .ToArray();
        return integrationEventsEntities.SelectMany(po => po.IntegrationEvents).ToList();
    }

    public void ClearIntegrationEvents()
    {
        var integrationEventsEntities = this._dbContext.ChangeTracker.Entries<BaseEntity>()
            .Select(po => po.Entity)
            .Where(po => po.IntegrationEvents.Any())
            .ToList();
        integrationEventsEntities
            .ForEach(entity => entity.ClearIntegrationEvents());
    }
}