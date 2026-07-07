namespace Hearth.Application.Common.Models;

public sealed record HouseholdDto(
    Guid Id,
    string Name,
    string AdultJoinCode,
    string ChildJoinCode);
