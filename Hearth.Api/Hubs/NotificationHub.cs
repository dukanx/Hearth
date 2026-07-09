using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Hearth.Api.Hubs;

// Server samo gura poruke ka klijentima (event "ReceiveNotification");
// klijent ne poziva ništa na serveru. Konekcije se prate radi presence-a
// (online korisnik ne dobija web push — već je dobio toast).
[Authorize]
public sealed class NotificationHub : Hub
{
    private readonly ConnectionTracker _tracker;

    public NotificationHub(ConnectionTracker tracker) => _tracker = tracker;

    public override Task OnConnectedAsync()
    {
        if (Guid.TryParse(Context.UserIdentifier, out var userId))
            _tracker.Connected(userId);

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        if (Guid.TryParse(Context.UserIdentifier, out var userId))
            _tracker.Disconnected(userId);

        return base.OnDisconnectedAsync(exception);
    }
}
