using Hearth.Domain.Enums;

namespace Hearth.Application.Common.Models;

public sealed record ShoppingItemDto(
    Guid Id,
    string Name,
    int Quantity,
    ShoppingItemStatus Status,
    Guid RequestedByUserId,
    Guid? BoughtByUserId,
    DateTime? BoughtAt,
    DateTime CreatedAt);
