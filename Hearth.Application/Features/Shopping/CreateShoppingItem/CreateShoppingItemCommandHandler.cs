using Hearth.Application.Common;
using Hearth.Application.Common.Interfaces;
using Hearth.Application.Common.Models;
using Hearth.Domain.Entities;
using MediatR;

namespace Hearth.Application.Features.Shopping.CreateShoppingItem;

public sealed class CreateShoppingItemCommandHandler
    : IRequestHandler<CreateShoppingItemCommand, Result<ShoppingItemDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public CreateShoppingItemCommandHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
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

        _db.ShoppingItems.Add(item);
        await _db.SaveChangesAsync(cancellationToken);

        return item.ToDto();
    }
}
