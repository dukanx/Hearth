using Hearth.Application.Common;
using Hearth.Application.Common.Interfaces;
using Hearth.Application.Common.Models;
using MediatR;

namespace Hearth.Application.Features.Shopping.GetShoppingItems;

public sealed class GetShoppingItemsQueryHandler
    : IRequestHandler<GetShoppingItemsQuery, Result<IReadOnlyList<ShoppingItemDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _currentUser;

    public GetShoppingItemsQueryHandler(IUnitOfWork uow, ICurrentUser currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<Result<IReadOnlyList<ShoppingItemDto>>> Handle(GetShoppingItemsQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.HouseholdId is not { } householdId)
            return Error.Forbidden("Nisi član nijednog domaćinstva.", "Household.NotMember");

        var shoppingItems = await _uow.ShoppingItems.ListAsync(
            i => i.HouseholdId == householdId
                 && (request.Status == null || i.Status == request.Status),
            cancellationToken);

        var items = shoppingItems
            .OrderByDescending(i => i.CreatedAt)
            .Select(i => i.ToDto())
            .ToList();

        return Result.Success<IReadOnlyList<ShoppingItemDto>>(items);
    }
}
