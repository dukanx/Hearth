using Hearth.Application.Common;
using Hearth.Application.Common.Interfaces;
using Hearth.Application.Common.Models;
using MediatR;

namespace Hearth.Application.Features.Households.GetMyHousehold;

public sealed class GetMyHouseholdQueryHandler
    : IRequestHandler<GetMyHouseholdQuery, Result<MyHouseholdDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _currentUser;

    public GetMyHouseholdQueryHandler(IUnitOfWork uow, ICurrentUser currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<Result<MyHouseholdDto>> Handle(GetMyHouseholdQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.HouseholdId is not { } householdId)
            return Error.Forbidden("Nisi član nijednog domaćinstva.", "Household.NotMember");

        var household = await _uow.Households.FirstOrDefaultAsync(
            h => h.Id == householdId, cancellationToken);

        if (household is null)
            return Error.NotFound("Domaćinstvo nije pronađeno.", "Household.NotFound");

        // Kodove za pozivanje vide samo odrasli.
        var isAdult = _currentUser.Role == Roles.Adult;

        return new MyHouseholdDto(
            household.Id,
            household.Name,
            isAdult ? household.AdultJoinCode : null,
            isAdult ? household.ChildJoinCode : null);
    }
}
