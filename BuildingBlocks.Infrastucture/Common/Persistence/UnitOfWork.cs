using Application.Common.Interfaces;
using Application.Common.Interfaces.Event;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Common.Persistence;

public class UnitOfWork:IUnitOfWork
{
    private readonly DbContext _context;
    private readonly IDomainEventDispatcher _domainEventsDispatcher;


    public UnitOfWork(
        DbContext context,
        IDomainEventDispatcher domainEventsDispatcher)
    {
        _context = context;
        _domainEventsDispatcher = domainEventsDispatcher;
    }

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        await _domainEventsDispatcher.Dispatch();

        return await _context.SaveChangesAsync(cancellationToken);
    }
}