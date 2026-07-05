using Hearth.Application.Common;
using Hearth.Application.Common.Interfaces;
using Hearth.Application.Common.Models;
using MediatR;

namespace Hearth.Application.Features.Auth.Login;

public sealed class LoginCommandHandler
    : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IIdentityService _identity;
    private readonly ITokenService _tokens;

    public LoginCommandHandler(IIdentityService identity, ITokenService tokens)
    {
        _identity = identity;
        _tokens = tokens;
    }

    public async Task<Result<AuthResponse>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var result = await _identity.ValidateCredentialsAsync(request.Email, request.Password);

        if (result.IsFailure)
            return Result.Failure<AuthResponse>(result.Error!);

        return _tokens.GenerateToken(result.Value);
    }
}
