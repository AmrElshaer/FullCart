using Application.Common.Interfaces;
using Application.Common.Interfaces.Authentication;
using Application.Common.models;
using Domain.Users;
using ErrorOr;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<User> _userManager;

    public IdentityService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ErrorOr<User>> CreateUserAsync(Email email, UserType userType, string password, string roleName)
    {
        var user = User.Create(Guid.NewGuid(), email, userType);

        if (user.IsError) return user.Errors;

        var result = await _userManager.CreateAsync(user.Value, password);

        if (!result.Succeeded) return ToError(result);

        var roleResult = await _userManager.AddToRolesAsync(user.Value, new[] { roleName });

        return !roleResult.Succeeded ? ToError(roleResult) : user;
    }

    public async Task<ErrorOr<UserDto>> GetUserAsync(Email email)
    {
        var user = await _userManager.FindByEmailAsync(email.Value);

        if (user is null) return Error.NotFound($"Not found user with email {email.Value}");

        var roles = await _userManager.GetRolesAsync(user);
        var userDto = new UserDto(user.Id, user.Email, user.UserType.ToString(), roles.ToList());
        return userDto;
    }

    private static Error ToError(IdentityResult result)
    {
        return Error.Validation(string.Join(',', result.Errors.Select(e => e.Description)));
    }
}