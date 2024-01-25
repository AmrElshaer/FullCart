using System.Security.Claims;
using Application.Common.Interfaces;
using Application.Common.models;
using Domain.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Infrastructure.Security.CurrentUserProvider;

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
        {
            throw new NullReferenceException();
        }

        var id = Guid.Parse(GetSingleClaimValue("id"));
        var userType= GetSingleClaimValue("userType");
        var roles = GetClaimValues(ClaimTypes.Role);
        var email = GetSingleClaimValue(ClaimTypes.Email);

        return new UserDto(id, email, userType,roles);
    }

    private List<string> GetClaimValues(string claimType) =>
        _httpContextAccessor.HttpContext!.User.Claims
            .Where(claim => claim.Type == claimType)
            .Select(claim => claim.Value)
            .ToList();

    private string GetSingleClaimValue(string claimType) =>
        _httpContextAccessor.HttpContext!.User.Claims
            .Single(claim => claim.Type == claimType)
            .Value;
}
