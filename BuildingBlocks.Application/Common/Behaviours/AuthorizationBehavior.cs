using System.Reflection;
using BuildingBlocks.Application.Common.Interfaces.Authentication;
using BuildingBlocks.Application.Security;
using ErrorOr;
using MediatR;

namespace BuildingBlocks.Application.Common.Behaviours;

public class
    AuthorizationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IAuthorizeRequest<TResponse>
    where TResponse : IErrorOr
{
    private readonly IAuthorizationService _authorizationService;

    public AuthorizationBehavior(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var authorizationAttributes = request.GetType()
            .GetCustomAttributes<AuthorizeAttribute>()
            .ToList();

        if (authorizationAttributes.Count == 0) return await next();


        var requiredRoles = authorizationAttributes
            .SelectMany(authorizationAttribute => authorizationAttribute.Roles?.Split(',') ?? Array.Empty<string>())
            .ToList();


        var authorizationResult = _authorizationService.AuthorizeCurrentUser(
            request,
            requiredRoles);

        return authorizationResult.IsError
            ? (dynamic)authorizationResult.Errors
            : await next();
    }
}