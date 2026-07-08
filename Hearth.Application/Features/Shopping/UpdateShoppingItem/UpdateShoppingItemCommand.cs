using Hearth.Application.Common;
using Hearth.Application.Common.Models;
using MediatR;

namespace Hearth.Application.Features.Shopping.UpdateShoppingItem;

public sealed record UpdateShoppingItemCommand(Guid ItemId, string Name, int Quantity)
    : IRequest<Result<ShoppingItemDto>>;
