using Application.Common.Interfaces;
using DotNetCore.CAP;

namespace Infrastructure.Common;

public class OutboxAccessor:IOutbox
{
    private readonly ICapPublisher _integrationEventPublisher;

    public OutboxAccessor(ICapPublisher integrationEventPublisher)
    {
        _integrationEventPublisher = integrationEventPublisher;
    }
}