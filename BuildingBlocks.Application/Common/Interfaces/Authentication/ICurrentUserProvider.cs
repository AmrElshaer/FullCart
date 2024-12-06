using BuildingBlocks.Application.Common.models;

namespace BuildingBlocks.Application.Common.Interfaces.Authentication;

public interface ICurrentUserProvider
{
    UserDto GetCurrentUser();
}