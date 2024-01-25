using Application.Common.models;
using Domain.Users;
using ErrorOr;

namespace Application.Common.Interfaces;

public interface IIdentityService
{
    Task<ErrorOr<User>> CreateUserAsync(Email email,UserType userType,string password,string roleName);
    Task<ErrorOr<UserDto>> GetUserAsync(Email email);
}
