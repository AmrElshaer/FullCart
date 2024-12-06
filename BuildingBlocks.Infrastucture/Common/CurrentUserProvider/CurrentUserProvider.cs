using System.Security.Claims;
using BuildingBlocks.Application.Common.Interfaces.Authentication;
using BuildingBlocks.Application.Common.models;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Infrastucture.Common.CurrentUserProvider;

public class CurrentUserProvider : ICurrentUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public UserDto GetCurrentUser()
    {
        if (_httpContextAccessor.HttpContext is null)
            throw new InvalidOperationException();

        if (_httpContextAccessor.HttpContext.User.Identity is not { IsAuthenticated: true })
            throw new UnauthorizedAccessException();

        var id = Guid.Parse(GetSingleClaimValue("id"));
        var userType = GetSingleClaimValue("userType");
        var roles = GetClaimValues(ClaimTypes.Role);
        var email = GetSingleClaimValue(ClaimTypes.Email);

        return new UserDto(id, email, userType, roles);
    }

    private List<string> GetClaimValues(string claimType)
    {
        return _httpContextAccessor.HttpContext!.User.Claims
            .Where(claim => claim.Type == claimType)
            .Select(claim => claim.Value)
            .ToList();
    }

    private string GetSingleClaimValue(string claimType)
    {
        return _httpContextAccessor.HttpContext!.User.Claims
            .Single(claim => claim.Type == claimType)
            .Value;
    }
}