using Domain.Users;

namespace Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken
    (
        Guid id,
        string firstName,
        string lastName,
        string email,
        UserType userType,
        List<string> roles
    );
}
