namespace Application.Common.Interfaces.Event;

public interface IIntegrationEventDispatcher
{
    Task Dispatch();
}