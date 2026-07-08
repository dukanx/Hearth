using Hearth.Application.Common;
using MediatR;

namespace Hearth.Application.Features.Notifications.MarkAllNotificationsRead;

public sealed record MarkAllNotificationsReadCommand : IRequest<Result>;
