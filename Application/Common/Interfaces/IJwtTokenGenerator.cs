using Domain.Users;

namespace Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken
    (
        Guid id,
        string email,
        string userType,
        List<string> roles
    );
}
