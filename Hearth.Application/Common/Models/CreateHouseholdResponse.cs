namespace Hearth.Application.Common.Models;

public sealed record CreateHouseholdResponse(AuthResponse Token, HouseholdDto Household);
