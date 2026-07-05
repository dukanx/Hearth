using Hearth.Application.Common.Models;

namespace Hearth.Application.Common.Interfaces;

public interface ITokenService
{
    AuthResponse GenerateToken(UserDto user);
}
