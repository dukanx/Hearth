using Hearth.Application.Common;
using Hearth.Application.Common.Interfaces;
using Hearth.Application.Common.Models;
using MediatR;

namespace Hearth.Application.Features.Auth.Register;

public sealed class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    private readonly IIdentityService _identity;
    private readonly ITokenService _tokens;

    public RegisterCommandHandler(IIdentityService identity, ITokenService tokens)
    {
        _identity = identity;
        _tokens = tokens;
    }

    public async Task<Result<AuthResponse>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var result = await _identity.CreateUserAsync(
            request.Email, request.Password, request.DisplayName);

        if (result.IsFailure)
            return Result.Failure<AuthResponse>(result.Error!);

        // Novi korisnik je u lobby stanju — token bez householdId/role.
        return _tokens.GenerateToken(result.Value);
    }
}
