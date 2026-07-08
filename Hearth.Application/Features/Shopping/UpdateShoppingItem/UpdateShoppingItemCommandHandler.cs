using Hearth.Application.Common;
using Hearth.Application.Common.Interfaces;
using Hearth.Application.Common.Models;
using MediatR;

namespace Hearth.Application.Features.Shopping.UpdateShoppingItem;

public sealed class UpdateShoppingItemCommandHandler
    : IRequestHandler<UpdateShoppingItemCommand, Result<ShoppingItemDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _currentUser;

    public UpdateShoppingItemCommandHandler(IUnitOfWork uow, ICurrentUser currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<Result<ShoppingItemDto>> Handle(UpdateShoppingItemCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.HouseholdId is not { } householdId)
            return Error.Forbidden("Nisi član nijednog domaćinstva.", "Household.NotMember");

        var item = await _uow.ShoppingItems.FirstOrDefaultAsync(
            i => i.Id == request.ItemId && i.HouseholdId == householdId, cancellationToken);

        if (item is null)
            return Error.NotFound("Stavka nije pronađena.", "Shopping.NotFound");

        item.Name = request.Name;
        item.Quantity = request.Quantity;

        _uow.ShoppingItems.Update(item);
        await _uow.SaveChangesAsync(cancellationToken);

        return item.ToDto();
    }
}
