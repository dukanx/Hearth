using Hearth.Application.Common.Models;

namespace Hearth.Application.Common.Interfaces;

// Realtime "push" ka jednom korisniku (sve njegove aktivne konekcije).
// Implementaciju daje host sloj (Api) preko SignalR IHubContext-a,
// pa Application ne zna za SignalR.
public interface IRealtimeNotifier
{
    Task SendToUserAsync(Guid userId, NotificationDto notification, CancellationToken cancellationToken = default);
}
