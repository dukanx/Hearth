using Hearth.Application.Common;
using MediatR;

namespace Hearth.Application.Features.Shopping.DeleteShoppingItem;

public sealed record DeleteShoppingItemCommand(Guid ItemId) : IRequest<Result>;
