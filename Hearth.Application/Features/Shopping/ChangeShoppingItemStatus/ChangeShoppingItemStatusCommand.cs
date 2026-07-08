using Hearth.Application.Common;
using Hearth.Application.Common.Models;
using Hearth.Domain.Enums;
using MediatR;

namespace Hearth.Application.Features.Shopping.ChangeShoppingItemStatus;

public sealed record ChangeShoppingItemStatusCommand(Guid ItemId, ShoppingItemStatus Status)
    : IRequest<Result<ShoppingItemDto>>;
