namespace Application.Common.Interfaces;

public interface IUnitOfWork
{
    Task<int> CommitAsync(
        CancellationToken cancellationToken = default);
}