using Hearth.Application.Common.Models;

namespace Hearth.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<Result<UserDto>> CreateUserAsync(string email, string password, string displayName);

    Task<Result<UserDto>> ValidateCredentialsAsync(string email, string password);
}
