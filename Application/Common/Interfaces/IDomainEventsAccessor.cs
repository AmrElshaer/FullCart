using Domain.Common;

namespace Application.Common.Interfaces;

public interface IDomainEventsAccessor
{
    IReadOnlyCollection<DomainEvent> GetAllDomainEvents();
    void ClearAllDomainEvents();
}