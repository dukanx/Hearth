using Hearth.Application.Common;
using Hearth.Application.Common.Models;
using MediatR;

namespace Hearth.Application.Features.Shopping.CreateShoppingItem;

public sealed record CreateShoppingItemCommand(string Name, int Quantity)
    : IRequest<Result<ShoppingItemDto>>;
