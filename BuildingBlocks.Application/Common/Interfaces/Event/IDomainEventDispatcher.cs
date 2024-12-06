namespace Application.Common.Interfaces.Event;

public interface IDomainEventDispatcher
{
    Task Dispatch();
}