using Hearth.Application.Common;
using MediatR;

namespace Hearth.Application.Features.Push.SubscribeToPush;

public sealed record SubscribeToPushCommand(string Endpoint, string P256dh, string Auth)
    : IRequest<Result>;
