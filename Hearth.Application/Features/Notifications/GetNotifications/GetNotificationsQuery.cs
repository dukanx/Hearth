using Hearth.Application.Common;
using Hearth.Application.Common.Models;
using MediatR;

namespace Hearth.Application.Features.Notifications.GetNotifications;

public sealed record GetNotificationsQuery(bool UnreadOnly)
    : IRequest<Result<IReadOnlyList<NotificationDto>>>;
