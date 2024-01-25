using Application.Security;
using ErrorOr;

namespace Application.Common.Interfaces;

public interface IAuthorizationService
{
    ErrorOr<Success> AuthorizeCurrentUser<T>(
        IAuthorizeRequest<T> request,
        List<string> requiredRoles);
}
