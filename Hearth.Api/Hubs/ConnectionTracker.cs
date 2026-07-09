using System.Collections.Concurrent;
using Hearth.Application.Common.Interfaces;

namespace Hearth.Api.Hubs;

// Broji aktivne SignalR konekcije po korisniku (više tabova/uređaja = više konekcija).
public sealed class ConnectionTracker : IRealtimePresence
{
    private readonly ConcurrentDictionary<Guid, int> _connections = new();

    public void Connected(Guid userId)
        => _connections.AddOrUpdate(userId, 1, (_, count) => count + 1);

    public void Disconnected(Guid userId)
    {
        while (_connections.TryGetValue(userId, out var count))
        {
            if (count <= 1)
            {
                if (_connections.TryRemove(new KeyValuePair<Guid, int>(userId, count)))
                    return;
            }
            else if (_connections.TryUpdate(userId, count - 1, count))
            {
                return;
            }
        }
    }

    public bool IsOnline(Guid userId) => _connections.ContainsKey(userId);
}
