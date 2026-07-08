using Hearth.Application.Common;
using Hearth.Application.Common.Interfaces;
using Hearth.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hearth.Application.Features.Shopping.GetShoppingItems;

public sealed class GetShoppingItemsQueryHandler
    : IRequestHandler<GetShoppingItemsQuery, Result<IReadOnlyList<ShoppingItemDto>>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public GetShoppingItemsQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<IReadOnlyList<ShoppingItemDto>>> Handle(GetShoppingItemsQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.HouseholdId is not { } householdId)
            return Error.Forbidden("Nisi član nijednog domaćinstva.", "Household.NotMember");

        var query = _db.ShoppingItems.Where(i => i.HouseholdId == householdId);

        if (request.Status is { } status)
            query = query.Where(i => i.Status == status);

        var items = await query
            .OrderByDescending(i => i.CreatedAt)
            .Select(i => new ShoppingItemDto(
                i.Id,
                i.Name,
                i.Quantity,
                i.Status,
                i.RequestedByUserId,
                i.BoughtByUserId,
                i.BoughtAt,
                i.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<ShoppingItemDto>>(items);
    }
}
