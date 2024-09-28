using Application.Common.models;

namespace Application.Common.Interfaces.Authentication;

public interface ICurrentUserProvider
{
    UserDto GetCurrentUser();
}