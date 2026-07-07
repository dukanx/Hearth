using Hearth.Application.Common;
using Hearth.Application.Common.Interfaces;
using Hearth.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hearth.Application.Features.Households.JoinHousehold;

public sealed class JoinHouseholdCommandHandler
    : IRequestHandler<JoinHouseholdCommand, Result<AuthResponse>>
{
    private readonly IApplicationDbContext _db;
    private readonly IIdentityService _identity;
    private readonly ITokenService _tokens;
    private readonly ICurrentUser _currentUser;

    public JoinHouseholdCommandHandler(
        IApplicationDbContext db,
        IIdentityService identity,
        ITokenService tokens,
        ICurrentUser currentUser)
    {
        _db = db;
        _identity = identity;
        _tokens = tokens;
        _currentUser = currentUser;
    }

    public async Task<Result<AuthResponse>> Handle(
        JoinHouseholdCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.Id is not { } userId)
            return Error.Unauthorized("Nedostaje identitet korisnika.", "Auth.NoIdentity");

        var userResult = await _identity.GetUserAsync(userId);
        if (userResult.IsFailure)
            return Result.Failure<AuthResponse>(userResult.Error!);

        if (userResult.Value.HouseholdId is not null)
            return Error.Conflict("Već si u domaćinstvu.", "Household.AlreadyMember");

        var code = request.JoinCode.Trim().ToUpperInvariant();

        var household = await _db.Households.FirstOrDefaultAsync(
            h => h.AdultJoinCode == code || h.ChildJoinCode == code, cancellationToken);

        if (household is null)
            return Error.NotFound("Nevažeći kod za pridruživanje.", "Household.CodeNotFound");

        // Uloga se određuje po tome koji je kod unet.
        var role = household.AdultJoinCode == code ? Roles.Adult : Roles.Child;

        var assigned = await _identity.AssignToHouseholdAsync(userId, household.Id, role);
        if (assigned.IsFailure)
            return Result.Failure<AuthResponse>(assigned.Error!);

        return _tokens.GenerateToken(assigned.Value);
    }
}
