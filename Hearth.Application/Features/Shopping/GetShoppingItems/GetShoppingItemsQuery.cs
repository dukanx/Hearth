using Hearth.Application.Common;
using Hearth.Application.Common.Models;
using Hearth.Domain.Enums;
using MediatR;

namespace Hearth.Application.Features.Shopping.GetShoppingItems;

public sealed record GetShoppingItemsQuery(ShoppingItemStatus? Status)
    : IRequest<Result<IReadOnlyList<ShoppingItemDto>>>;
