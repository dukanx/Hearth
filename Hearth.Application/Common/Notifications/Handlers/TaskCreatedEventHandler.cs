using Hearth.Application.Common.Interfaces;
using Hearth.Application.Common.Notifications.Events;
using MediatR;

namespace Hearth.Application.Common.Notifications.Handlers;

public sealed class TaskCreatedEventHandler : INotificationHandler<TaskCreatedEvent>
{
    private readonly NotificationBroadcaster _broadcaster;
    private readonly IIdentityService _identity;

    public TaskCreatedEventHandler(NotificationBroadcaster broadcaster, IIdentityService identity)
    {
        _broadcaster = broadcaster;
        _identity = identity;
    }

    public async Task Handle(TaskCreatedEvent notification, CancellationToken cancellationToken)
    {
        var actor = await _identity.GetUserAsync(notification.ActorId);
        var actorName = actor.IsSuccess ? actor.Value.DisplayName : "Neko";

        // Ako je zadatak odmah dodeljen nekom drugom — taj dobija ličnu poruku.
        var excluded = new List<Guid> { notification.ActorId };
        if (notification.AssignedToUserId is { } assigneeId)
        {
            excluded.Add(assigneeId);

            if (assigneeId != notification.ActorId)
                await _broadcaster.SendToUserAsync(
                    notification.HouseholdId,
                    assigneeId,
                    $"{actorName} ti je dodelio/la zadatak: „{notification.TaskTitle}\".",
                    cancellationToken);
        }

        // Ostali članovi (sem aktera i dodeljenog) dobijaju opšte obaveštenje.
        await _broadcaster.BroadcastToHouseholdAsync(
            notification.HouseholdId,
            excluded,
            $"{actorName} je dodao/la zadatak: „{notification.TaskTitle}\".",
            cancellationToken);
    }
}
