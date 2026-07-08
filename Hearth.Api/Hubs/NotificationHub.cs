using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Hearth.Api.Hubs;

// Server samo gura poruke ka klijentima (event "ReceiveNotification");
// klijent ne poziva ništa na serveru, pa hub nema metode.
[Authorize]
public sealed class NotificationHub : Hub
{
}
