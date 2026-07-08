using Hearth.Application.Common.Interfaces;
using Hearth.Application.Common.Notifications.Events;
using MediatR;

namespace Hearth.Application.Common.Notifications.Handlers;

public sealed class ShoppingItemAddedEventHandler : INotificationHandler<ShoppingItemAddedEvent>
{
    private readonly NotificationBroadcaster _broadcaster;
    private readonly IIdentityService _identity;

    public ShoppingItemAddedEventHandler(NotificationBroadcaster broadcaster, IIdentityService identity)
    {
        _broadcaster = broadcaster;
        _identity = identity;
    }

    public async Task Handle(ShoppingItemAddedEvent notification, CancellationToken cancellationToken)
    {
        var actor = await _identity.GetUserAsync(notification.ActorId);
        var actorName = actor.IsSuccess ? actor.Value.DisplayName : "Neko";

        await _broadcaster.BroadcastToHouseholdAsync(
            notification.HouseholdId,
            new[] { notification.ActorId },
            $"{actorName} je dodao/la „{notification.ItemName}\" na listu za kupovinu.",
            cancellationToken);
    }
}
