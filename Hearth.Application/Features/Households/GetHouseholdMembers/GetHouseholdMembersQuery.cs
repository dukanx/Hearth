using Hearth.Application.Common;
using Hearth.Application.Common.Models;
using MediatR;

namespace Hearth.Application.Features.Households.GetHouseholdMembers;

public sealed record GetHouseholdMembersQuery : IRequest<Result<IReadOnlyList<HouseholdMemberDto>>>;
