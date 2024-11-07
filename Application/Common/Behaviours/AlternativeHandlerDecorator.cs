using System.Reflection;
using Application.Common.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;

namespace Application.Common.Behaviours;

public class AlternativeHandlerDecorator<TRequest, TResponse>
    : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IFeatureManager _featureManager;
    private readonly IRequestHandler<TRequest, TResponse> _handler;
    private readonly ILogger<AlternativeHandlerDecorator<TRequest, TResponse>> _logger;
    private readonly IServiceProvider _serviceProvider;

    public AlternativeHandlerDecorator(IFeatureManager featureManager, IRequestHandler<TRequest, TResponse> handler,
        ILogger<AlternativeHandlerDecorator<TRequest, TResponse>> logger, IServiceProvider serviceProvider)
    {
        _featureManager = featureManager;
        _handler = handler;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var featureFlag = GetFeatureFlag();
        if (featureFlag == null) return await _handler.Handle(request, cancellationToken);

        if (!await _featureManager.IsEnabledAsync(featureFlag.FeatureFlag))
            return await _handler.Handle(request, cancellationToken);
        _logger.LogInformation("Feature flag enabled: {FeatureFlag},{HandlerAlternative}", featureFlag.FeatureFlag,
            featureFlag.AlternativeHandlerType);

        var alternativeHandler = GetAlternativeHandler(featureFlag.AlternativeHandlerType);
        return await alternativeHandler.Handle(request, cancellationToken);
    }

    private static FeatureFlagAttribute? GetFeatureFlag()
    {
        return typeof(TRequest).GetCustomAttribute<FeatureFlagAttribute>();
    }

    private IRequestHandler<TRequest, TResponse> GetAlternativeHandler(Type alternativeHandlerType)
    {
        return (IRequestHandler<TRequest, TResponse>)ActivatorUtilities.CreateInstance(_serviceProvider,
            alternativeHandlerType);
    }
}