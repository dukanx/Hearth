using Hearth.Application.Common.Models;
using Hearth.Domain.Entities;

namespace Hearth.Application.Features.Shopping;

internal static class ShoppingItemMappings
{
    public static ShoppingItemDto ToDto(this ShoppingItem item) => new(
        item.Id,
        item.Name,
        item.Quantity,
        item.Status,
        item.RequestedByUserId,
        item.BoughtByUserId,
        item.BoughtAt,
        item.CreatedAt);
}
