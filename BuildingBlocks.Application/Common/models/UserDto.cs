

namespace BuildingBlocks.Application.Common.models;

public record UserDto(
    Guid Id,
    string Email,
    string UserType,
    IReadOnlyList<string> Roles);
