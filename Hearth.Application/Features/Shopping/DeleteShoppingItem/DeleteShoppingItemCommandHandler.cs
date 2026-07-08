using Hearth.Application.Common;
using Hearth.Application.Common.Interfaces;
using MediatR;

namespace Hearth.Application.Features.Shopping.DeleteShoppingItem;

public sealed class DeleteShoppingItemCommandHandler : IRequestHandler<DeleteShoppingItemCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _currentUser;

    public DeleteShoppingItemCommandHandler(IUnitOfWork uow, ICurrentUser currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(DeleteShoppingItemCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.HouseholdId is not { } householdId)
            return Result.Failure(Error.Forbidden("Nisi član nijednog domaćinstva.", "Household.NotMember"));

        var item = await _uow.ShoppingItems.FirstOrDefaultAsync(
            i => i.Id == request.ItemId && i.HouseholdId == householdId, cancellationToken);

        if (item is null)
            return Result.Failure(Error.NotFound("Stavka nije pronađena.", "Shopping.NotFound"));

        _uow.ShoppingItems.Remove(item);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
