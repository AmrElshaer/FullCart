using BuildingBlocks.Domain.Common;

namespace BuildingBlocks.Application.Common.Interfaces;

public interface IIntegrationEventsAccessor
{
    IReadOnlyCollection<IntegrationEvent> GetIntegrationEvents();
    void ClearIntegrationEvents();
}