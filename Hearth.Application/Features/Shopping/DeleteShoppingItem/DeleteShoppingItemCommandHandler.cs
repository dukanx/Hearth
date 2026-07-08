using Hearth.Application.Common;
using Hearth.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hearth.Application.Features.Shopping.DeleteShoppingItem;

public sealed class DeleteShoppingItemCommandHandler : IRequestHandler<DeleteShoppingItemCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public DeleteShoppingItemCommandHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(DeleteShoppingItemCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.HouseholdId is not { } householdId)
            return Result.Failure(Error.Forbidden("Nisi član nijednog domaćinstva.", "Household.NotMember"));

        var item = await _db.ShoppingItems.FirstOrDefaultAsync(
            i => i.Id == request.ItemId && i.HouseholdId == householdId, cancellationToken);

        if (item is null)
            return Result.Failure(Error.NotFound("Stavka nije pronađena.", "Shopping.NotFound"));

        _db.ShoppingItems.Remove(item);
        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
