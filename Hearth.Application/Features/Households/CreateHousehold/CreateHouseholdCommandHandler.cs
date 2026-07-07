using Hearth.Application.Common;
using Hearth.Application.Common.Interfaces;
using Hearth.Application.Common.Models;
using Hearth.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hearth.Application.Features.Households.CreateHousehold;

public sealed class CreateHouseholdCommandHandler
    : IRequestHandler<CreateHouseholdCommand, Result<CreateHouseholdResponse>>
{
    private readonly IApplicationDbContext _db;
    private readonly IIdentityService _identity;
    private readonly ITokenService _tokens;
    private readonly IJoinCodeGenerator _codes;
    private readonly ICurrentUser _currentUser;

    public CreateHouseholdCommandHandler(
        IApplicationDbContext db,
        IIdentityService identity,
        ITokenService tokens,
        IJoinCodeGenerator codes,
        ICurrentUser currentUser)
    {
        _db = db;
        _identity = identity;
        _tokens = tokens;
        _codes = codes;
        _currentUser = currentUser;
    }

    public async Task<Result<CreateHouseholdResponse>> Handle(
        CreateHouseholdCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.Id is not { } userId)
            return Error.Unauthorized("Nedostaje identitet korisnika.", "Auth.NoIdentity");

        // Lobby provera pre kreiranja — da ne ostane siroto domaćinstvo ako korisnik već ima jedno.
        var userResult = await _identity.GetUserAsync(userId);
        if (userResult.IsFailure)
            return Result.Failure<CreateHouseholdResponse>(userResult.Error!);

        if (userResult.Value.HouseholdId is not null)
            return Error.Conflict("Već si u domaćinstvu.", "Household.AlreadyMember");

        var adultCode = await GenerateUniqueCodeAsync(cancellationToken);
        var childCode = await GenerateUniqueCodeAsync(cancellationToken, exclude: adultCode);

        var household = new Household
        {
            Name = request.Name,
            AdultJoinCode = adultCode,
            ChildJoinCode = childCode
        };

        _db.Households.Add(household);
        await _db.SaveChangesAsync(cancellationToken);

        var assigned = await _identity.AssignToHouseholdAsync(userId, household.Id, Roles.Adult);
        if (assigned.IsFailure)
            return Result.Failure<CreateHouseholdResponse>(assigned.Error!);

        var token = _tokens.GenerateToken(assigned.Value);
        var dto = new HouseholdDto(
            household.Id, household.Name, household.AdultJoinCode, household.ChildJoinCode);

        return new CreateHouseholdResponse(token, dto);
    }

    // Kod mora biti jedinstven preko OBE kolone svih domaćinstava (adult i child),
    // inače je unos koda pri pridruživanju dvosmislen.
    private async Task<string> GenerateUniqueCodeAsync(CancellationToken ct, string? exclude = null)
    {
        while (true)
        {
            var code = _codes.Generate();
            if (code == exclude)
                continue;

            var taken = await _db.Households.AnyAsync(
                h => h.AdultJoinCode == code || h.ChildJoinCode == code, ct);

            if (!taken)
                return code;
        }
    }
}
