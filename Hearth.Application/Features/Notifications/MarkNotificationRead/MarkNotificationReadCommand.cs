using Hearth.Application.Common;
using MediatR;

namespace Hearth.Application.Features.Notifications.MarkNotificationRead;

public sealed record MarkNotificationReadCommand(Guid NotificationId) : IRequest<Result>;
