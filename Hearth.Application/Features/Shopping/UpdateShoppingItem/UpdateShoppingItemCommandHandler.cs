using Hearth.Application.Common;
using Hearth.Application.Common.Interfaces;
using Hearth.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hearth.Application.Features.Shopping.UpdateShoppingItem;

public sealed class UpdateShoppingItemCommandHandler
    : IRequestHandler<UpdateShoppingItemCommand, Result<ShoppingItemDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public UpdateShoppingItemCommandHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<ShoppingItemDto>> Handle(UpdateShoppingItemCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.HouseholdId is not { } householdId)
            return Error.Forbidden("Nisi član nijednog domaćinstva.", "Household.NotMember");

        var item = await _db.ShoppingItems.FirstOrDefaultAsync(
            i => i.Id == request.ItemId && i.HouseholdId == householdId, cancellationToken);

        if (item is null)
            return Error.NotFound("Stavka nije pronađena.", "Shopping.NotFound");

        item.Name = request.Name;
        item.Quantity = request.Quantity;

        await _db.SaveChangesAsync(cancellationToken);

        return item.ToDto();
    }
}
