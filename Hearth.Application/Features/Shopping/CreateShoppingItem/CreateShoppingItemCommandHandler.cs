using Hearth.Application.Common;
using Hearth.Application.Common.Interfaces;
using Hearth.Application.Common.Models;
using Hearth.Application.Common.Notifications.Events;
using Hearth.Domain.Entities;
using MediatR;

namespace Hearth.Application.Features.Shopping.CreateShoppingItem;

public sealed class CreateShoppingItemCommandHandler
    : IRequestHandler<CreateShoppingItemCommand, Result<ShoppingItemDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _currentUser;
    private readonly IPublisher _publisher;

    public CreateShoppingItemCommandHandler(IUnitOfWork uow, ICurrentUser currentUser, IPublisher publisher)
    {
        _uow = uow;
        _currentUser = currentUser;
        _publisher = publisher;
    }

    public async Task<Result<ShoppingItemDto>> Handle(CreateShoppingItemCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.Id is not { } userId)
            return Error.Unauthorized("Nedostaje identitet korisnika.", "Auth.NoIdentity");

        if (_currentUser.HouseholdId is not { } householdId)
            return Error.Forbidden("Nisi član nijednog domaćinstva.", "Household.NotMember");

        var item = new ShoppingItem
        {
            Name = request.Name,
            Quantity = request.Quantity,
            HouseholdId = householdId,
            RequestedByUserId = userId
        };

        _uow.ShoppingItems.Add(item);
        await _uow.SaveChangesAsync(cancellationToken);

        // Tek posle uspešnog commita — obavesti ostale članove.
        await _publisher.Publish(
            new ShoppingItemAddedEvent(householdId, userId, item.Name), cancellationToken);

        return item.ToDto();
    }
}
