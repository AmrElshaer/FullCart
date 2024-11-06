using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviours;

public class BenchmarkBehavior<TRequest, TResponse>(ILogger<BenchmarkBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Start timing the request
        var stopwatch = Stopwatch.StartNew();

        // Process the request
        var response = await next();

        // Stop timing and evaluate the duration
        stopwatch.Stop();
        var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;


        logger.LogInformation("request: {Request} took {ElapsedMilliseconds} ms.",
            typeof(TRequest).Name, elapsedMilliseconds);

        return response;
    }
}