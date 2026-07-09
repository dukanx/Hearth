namespace Hearth.Application.Common.Models;

// Kodovi su null za decu — pozivanje novih članova je posao odraslih.
public sealed record MyHouseholdDto(
    Guid Id,
    string Name,
    string? AdultJoinCode,
    string? ChildJoinCode);
