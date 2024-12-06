using BuildingBlocks.Domain.Common;

namespace BuildingBlocks.Application.Common.Interfaces;

public interface IDomainEventsAccessor
{
    IReadOnlyCollection<DomainEvent> GetAllDomainEvents();
    void ClearAllDomainEvents();
}