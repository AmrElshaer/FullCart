using Domain.Common;

namespace Application.Common.Interfaces;

public interface IIntegrationEventsAccessor
{
    IReadOnlyCollection<IntegrationEvent> GetIntegrationEvents();
    void ClearIntegrationEvents();
}