using Hearth.Application.Common.Models;

namespace Hearth.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<Result<UserDto>> CreateUserAsync(string email, string password, string displayName);

    Task<Result<UserDto>> ValidateCredentialsAsync(string email, string password);

    Task<Result<UserDto>> GetUserAsync(Guid userId);

    Task<Result<UserDto>> AssignToHouseholdAsync(Guid userId, Guid householdId, string role);

    // Id-jevi svih članova domaćinstva — za razašiljanje obaveštenja.
    Task<Result<IReadOnlyList<Guid>>> GetHouseholdMemberIdsAsync(Guid householdId);
}
