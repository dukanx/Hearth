namespace Hearth.Application.Common.Models;

public sealed record UserDto(
    Guid Id,
    string Email,
    string DisplayName,
    Guid? HouseholdId,
    string? Role);
