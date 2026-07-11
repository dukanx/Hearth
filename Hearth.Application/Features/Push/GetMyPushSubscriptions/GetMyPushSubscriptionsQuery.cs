using Hearth.Application.Common;
using Hearth.Application.Common.Models;
using MediatR;

namespace Hearth.Application.Features.Push.GetMyPushSubscriptions;

public sealed record GetMyPushSubscriptionsQuery
    : IRequest<Result<IReadOnlyList<PushSubscriptionDto>>>;
