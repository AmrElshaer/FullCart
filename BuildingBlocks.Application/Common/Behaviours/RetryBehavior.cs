using MediatR;
using Microsoft.EntityFrameworkCore;
using Polly;

namespace BuildingBlocks.Application.Common.Behaviours;

public class RetryBehavior<TRequest, TResponse>: IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IAsyncPolicy _retryPolicy;

    public RetryBehavior()
    {
        _retryPolicy = Policy.Handle<DbUpdateConcurrencyException>()
            .WaitAndRetryAsync(3,
                i => TimeSpan.FromSeconds(i));
    }
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync( async () => await next());
    }
}
