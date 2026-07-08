using Hearth.Application.Common;
using Hearth.Application.Common.Interfaces;
using Hearth.Application.Common.Models;
using Hearth.Application.Common.Notifications.Events;
using Hearth.Domain.Enums;
using MediatR;

namespace Hearth.Application.Features.Shopping.ChangeShoppingItemStatus;

public sealed class ChangeShoppingItemStatusCommandHandler
    : IRequestHandler<ChangeShoppingItemStatusCommand, Result<ShoppingItemDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _currentUser;
    private readonly IPublisher _publisher;

    public ChangeShoppingItemStatusCommandHandler(IUnitOfWork uow, ICurrentUser currentUser, IPublisher publisher)
    {
        _uow = uow;
        _currentUser = currentUser;
        _publisher = publisher;
    }

    public async Task<Result<ShoppingItemDto>> Handle(ChangeShoppingItemStatusCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.Id is not { } userId)
            return Error.Unauthorized("Nedostaje identitet korisnika.", "Auth.NoIdentity");

        if (_currentUser.HouseholdId is not { } householdId)
            return Error.Forbidden("Nisi član nijednog domaćinstva.", "Household.NotMember");

        var item = await _uow.ShoppingItems.FirstOrDefaultAsync(
            i => i.Id == request.ItemId && i.HouseholdId == householdId, cancellationToken);

        if (item is null)
            return Error.NotFound("Stavka nije pronađena.", "Shopping.NotFound");

        if (item.Status == request.Status)
            return Error.Validation("Stavka je već u tom statusu.", "Shopping.SameStatus");

        // Rich domain: postavlja/čisti BoughtByUserId + BoughtAt (podatak za notifikacije).
        if (request.Status == ShoppingItemStatus.Bought)
            item.MarkBought(userId);
        else
            item.MarkNeeded();

        _uow.ShoppingItems.Update(item);
        await _uow.SaveChangesAsync(cancellationToken);

        // Obaveštavamo samo o kupovini (ne o vraćanju u Needed), tek posle commita.
        if (request.Status == ShoppingItemStatus.Bought)
            await _publisher.Publish(
                new ShoppingItemBoughtEvent(householdId, userId, item.Name), cancellationToken);

        return item.ToDto();
    }
}
