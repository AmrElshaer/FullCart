namespace BuildingBlocks.Application.Common.Interfaces.Authentication;

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