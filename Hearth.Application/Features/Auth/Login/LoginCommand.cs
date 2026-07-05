using Hearth.Application.Common;
using Hearth.Application.Common.Models;
using MediatR;

namespace Hearth.Application.Features.Auth.Login;

public sealed record LoginCommand(string Email, string Password)
    : IRequest<Result<AuthResponse>>;
