using Hearth.Application.Common;
using Hearth.Application.Common.Models;
using MediatR;

namespace Hearth.Application.Features.Households.GetMyHousehold;

public sealed record GetMyHouseholdQuery : IRequest<Result<MyHouseholdDto>>;
