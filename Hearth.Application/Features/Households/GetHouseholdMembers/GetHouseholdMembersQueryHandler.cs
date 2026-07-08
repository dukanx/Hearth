using Hearth.Application.Common;
using Hearth.Application.Common.Interfaces;
using Hearth.Application.Common.Models;
using MediatR;

namespace Hearth.Application.Features.Households.GetHouseholdMembers;

public sealed class GetHouseholdMembersQueryHandler
    : IRequestHandler<GetHouseholdMembersQuery, Result<IReadOnlyList<HouseholdMemberDto>>>
{
    private readonly IIdentityService _identity;
    private readonly ICurrentUser _currentUser;

    public GetHouseholdMembersQueryHandler(IIdentityService identity, ICurrentUser currentUser)
    {
        _identity = identity;
        _currentUser = currentUser;
    }

    public async Task<Result<IReadOnlyList<HouseholdMemberDto>>> Handle(
        GetHouseholdMembersQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.HouseholdId is not { } householdId)
            return Error.Forbidden("Nisi član nijednog domaćinstva.", "Household.NotMember");

        var members = await _identity.GetHouseholdMembersAsync(householdId);
        if (members.IsFailure)
            return Result.Failure<IReadOnlyList<HouseholdMemberDto>>(members.Error!);

        var dtos = members.Value
            .Select(m => new HouseholdMemberDto(m.Id, m.DisplayName, m.Role))
            .ToList();

        return Result.Success<IReadOnlyList<HouseholdMemberDto>>(dtos);
    }
}
