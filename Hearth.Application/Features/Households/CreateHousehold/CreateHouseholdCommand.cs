using Hearth.Application.Common;
using Hearth.Application.Common.Models;
using MediatR;

namespace Hearth.Application.Features.Households.CreateHousehold;

public sealed record CreateHouseholdCommand(string Name)
    : IRequest<Result<CreateHouseholdResponse>>;
