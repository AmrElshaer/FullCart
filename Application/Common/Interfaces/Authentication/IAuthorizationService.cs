using Application.Security;

namespace Application.Common.Interfaces.Authentication;

public interface IAuthorizationService
{
    ErrorOr<Success> AuthorizeCurrentUser<T>(
        IAuthorizeRequest<T> request,
        List<string> requiredRoles);
}