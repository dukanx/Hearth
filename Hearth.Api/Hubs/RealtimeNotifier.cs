using Hearth.Application.Common.Interfaces;
using Hearth.Application.Common.Models;
using Microsoft.AspNetCore.SignalR;

namespace Hearth.Api.Hubs;

// Implementacija Application apstrakcije preko SignalR-a.
// Živi u host sloju (Api) da Infrastructure ne mora da zna za SignalR.
public sealed class RealtimeNotifier : IRealtimeNotifier
{
    private readonly IHubContext<NotificationHub> _hub;

    public RealtimeNotifier(IHubContext<NotificationHub> hub) => _hub = hub;

    public Task SendToUserAsync(Guid userId, NotificationDto notification, CancellationToken cancellationToken = default)
        => _hub.Clients.User(userId.ToString())
            .SendAsync("ReceiveNotification", notification, cancellationToken);
}
