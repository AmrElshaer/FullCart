using BuildingBlocks.Application.Security;
using ErrorOr;

namespace BuildingBlocks.Application.Common.Interfaces.Authentication;

public interface IAuthorizationService
{
    ErrorOr<Success> AuthorizeCurrentUser<T>(
        IAuthorizeRequest<T> request,
        List<string> requiredRoles);
}