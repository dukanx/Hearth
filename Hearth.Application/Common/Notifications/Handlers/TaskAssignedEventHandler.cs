using Hearth.Application.Common.Interfaces;
using Hearth.Application.Common.Notifications.Events;
using MediatR;

namespace Hearth.Application.Common.Notifications.Handlers;

public sealed class TaskAssignedEventHandler : INotificationHandler<TaskAssignedEvent>
{
    private readonly NotificationBroadcaster _broadcaster;
    private readonly IIdentityService _identity;

    public TaskAssignedEventHandler(NotificationBroadcaster broadcaster, IIdentityService identity)
    {
        _broadcaster = broadcaster;
        _identity = identity;
    }

    public async Task Handle(TaskAssignedEvent notification, CancellationToken cancellationToken)
    {
        // Dodela sebi ne generiše obaveštenje.
        if (notification.AssigneeId == notification.ActorId)
            return;

        var actor = await _identity.GetUserAsync(notification.ActorId);
        var actorName = actor.IsSuccess ? actor.Value.DisplayName : "Neko";

        await _broadcaster.SendToUserAsync(
            notification.HouseholdId,
            notification.AssigneeId,
            $"{actorName} ti je dodelio/la zadatak: „{notification.TaskTitle}\".",
            cancellationToken);
    }
}
