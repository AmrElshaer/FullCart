using System.Reflection;
using Application.Common.Attributes;
using Mapster;
using Microsoft.FeatureManagement;

namespace Application.Common.Behaviours;

public class FeatureFlagBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IFeatureManager _featureManager;
    private readonly IMediator _mediator;

    public FeatureFlagBehavior(IFeatureManager featureManager, IMediator mediator)
    {
        _featureManager = featureManager;
        _mediator = mediator;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var featureFlagAttribute = typeof(TRequest).GetCustomAttribute<FeatureFlagAttribute>();

        if (featureFlagAttribute != null)
        {
            var featureFlag = featureFlagAttribute.FeatureFlag;
            var alternativeHandlerType = featureFlagAttribute.AlternativeHandlerType;

            if (await _featureManager.IsEnabledAsync(featureFlag))
            {
                var handlerInterface = alternativeHandlerType.GetInterfaces()
                    .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));
                var alternativeRequestType = handlerInterface.GetGenericArguments()[0];
                var mappedRequest = request.Adapt(typeof(TRequest), alternativeRequestType);
                var result = await _mediator.Send(mappedRequest!, cancellationToken);
                return (TResponse)result!;
            }
        }

        return await next();
    }
}