using Hearth.Application.Common;
using Hearth.Application.Common.Models;
using MediatR;

namespace Hearth.Application.Features.Households.JoinHousehold;

public sealed record JoinHouseholdCommand(string JoinCode)
    : IRequest<Result<AuthResponse>>;
