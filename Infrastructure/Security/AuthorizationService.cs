using Application.Common.Interfaces;
using Application.Security;
using ErrorOr;
using Infrastructure.Security.CurrentUserProvider;

namespace Infrastructure.Security;

public class AuthorizationService
    : IAuthorizationService
{
    private readonly ICurrentUserProvider _currentUserProvider;

    public AuthorizationService(ICurrentUserProvider currentUserProvider)
    {
        _currentUserProvider = currentUserProvider;
    }
    public ErrorOr<Success> AuthorizeCurrentUser<T>(
        IAuthorizeRequest<T> request,
        List<string> requiredRoles)
    {
        var currentUser = _currentUserProvider.GetCurrentUser();

        if (requiredRoles.Except(currentUser.Roles).Any())
        {
            return Error.Unauthorized(description: "User is missing required roles for taking this action");
        }

        

        return Result.Success;
    }
}
