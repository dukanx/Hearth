using Hearth.Application.Common;
using Hearth.Application.Common.Models;
using MediatR;

namespace Hearth.Application.Features.Auth.Register;

public sealed record RegisterCommand(string Email, string Password, string DisplayName)
    : IRequest<Result<AuthResponse>>;
