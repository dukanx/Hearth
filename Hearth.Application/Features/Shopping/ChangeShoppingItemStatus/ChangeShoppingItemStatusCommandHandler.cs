using Hearth.Application.Common;
using Hearth.Application.Common.Interfaces;
using Hearth.Application.Common.Models;
using Hearth.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hearth.Application.Features.Shopping.ChangeShoppingItemStatus;

public sealed class ChangeShoppingItemStatusCommandHandler
    : IRequestHandler<ChangeShoppingItemStatusCommand, Result<ShoppingItemDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public ChangeShoppingItemStatusCommandHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<ShoppingItemDto>> Handle(ChangeShoppingItemStatusCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.Id is not { } userId)
            return Error.Unauthorized("Nedostaje identitet korisnika.", "Auth.NoIdentity");

        if (_currentUser.HouseholdId is not { } householdId)
            return Error.Forbidden("Nisi član nijednog domaćinstva.", "Household.NotMember");

        var item = await _db.ShoppingItems.FirstOrDefaultAsync(
            i => i.Id == request.ItemId && i.HouseholdId == householdId, cancellationToken);

        if (item is null)
            return Error.NotFound("Stavka nije pronađena.", "Shopping.NotFound");

        if (item.Status == request.Status)
            return Error.Validation("Stavka je već u tom statusu.", "Shopping.SameStatus");

        // Rich domain: postavlja/čisti BoughtByUserId + BoughtAt (podatak za Blok E).
        if (request.Status == ShoppingItemStatus.Bought)
            item.MarkBought(userId);
        else
            item.MarkNeeded();

        await _db.SaveChangesAsync(cancellationToken);

        return item.ToDto();
    }
}
