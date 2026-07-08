using Hearth.Application.Common;
using MediatR;

namespace Hearth.Application.Features.Push.UnsubscribeFromPush;

public sealed record UnsubscribeFromPushCommand(string Endpoint) : IRequest<Result>;
