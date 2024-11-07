using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Application.Common.Behaviours;

internal sealed class RequestLoggingPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
    where TResponse : IErrorOr
{
    private readonly ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> _logger;

    public RequestLoggingPipelineBehavior(ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation(
            "Processing request {RequestName}", requestName);

        var result = await next();

        if (result.IsError)
            using (LogContext.PushProperty("Error", result.Errors, true))
            {
                _logger.LogError(
                    "Completed request {RequestName} with error", requestName);
            }
        else
            _logger.LogInformation(
                "Completed request {RequestName}", requestName);

        return result;
    }
}