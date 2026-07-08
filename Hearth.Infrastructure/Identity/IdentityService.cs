using Hearth.Application.Common;
using Hearth.Application.Common.Interfaces;
using Hearth.Application.Common.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Hearth.Infrastructure.Identity;

public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(UserManager<ApplicationUser> userManager)
        => _userManager = userManager;

    public async Task<Result<UserDto>> CreateUserAsync(string email, string password, string displayName)
    {
        var existing = await _userManager.FindByEmailAsync(email);
        if (existing is not null)
            return Error.Conflict("Korisnik sa tim email-om već postoji.", "Auth.EmailTaken");

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            DisplayName = displayName
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var message = string.Join("; ", result.Errors.Select(e => e.Description));
            return Error.Validation(message, "Auth.CreateFailed");
        }

        return await MapAsync(user);
    }

    public async Task<Result<UserDto>> ValidateCredentialsAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, password))
            return Error.Unauthorized("Neispravan email ili lozinka.", "Auth.InvalidCredentials");

        return await MapAsync(user);
    }

    public async Task<Result<UserDto>> GetUserAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Error.NotFound("Korisnik nije pronađen.", "Auth.UserNotFound");

        return await MapAsync(user);
    }

    public async Task<Result<UserDto>> AssignToHouseholdAsync(Guid userId, Guid householdId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Error.NotFound("Korisnik nije pronađen.", "Auth.UserNotFound");

        // Odbrambena provera lobby stanja (i ako je pozivalac već proverio).
        if (user.HouseholdId is not null)
            return Error.Conflict("Već si u domaćinstvu.", "Household.AlreadyMember");

        user.HouseholdId = householdId;
        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return Error.Validation(
                string.Join("; ", updateResult.Errors.Select(e => e.Description)), "Auth.UpdateFailed");

        var roleResult = await _userManager.AddToRoleAsync(user, role);
        if (!roleResult.Succeeded)
            return Error.Validation(
                string.Join("; ", roleResult.Errors.Select(e => e.Description)), "Auth.RoleAssignFailed");

        return await MapAsync(user);
    }

    public async Task<Result<IReadOnlyList<Guid>>> GetHouseholdMemberIdsAsync(Guid householdId)
    {
        var ids = await _userManager.Users
            .Where(u => u.HouseholdId == householdId)
            .Select(u => u.Id)
            .ToListAsync();

        return Result.Success<IReadOnlyList<Guid>>(ids);
    }

    private async Task<UserDto> MapAsync(ApplicationUser user)
    {
        // Korisnik u ovom domenu ima najviše jednu ulogu (Adult xor Child);
        // u lobby stanju nema nijednu -> Role == null.
        var roles = await _userManager.GetRolesAsync(user);

        return new UserDto(
            user.Id,
            user.Email!,
            user.DisplayName,
            user.HouseholdId,
            roles.FirstOrDefault());
    }
}
