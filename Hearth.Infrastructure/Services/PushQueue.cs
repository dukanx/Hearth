using System.Threading.Channels;
using Hearth.Application.Common.Interfaces;

namespace Hearth.Infrastructure.Services;

// Web push se NE šalje u okviru HTTP zahteva — spoljni push servisi (Apple/Google)
// umeju da budu spori ili nedostupni, a korisnik ne sme da čeka na njih.
// Handleri preko IPushNotifier samo ubace poruku u red; PushSenderService je šalje.
// Red je in-memory: restart servera izgubi neposlate push-eve, što je za
// best-effort notifikacije prihvatljivo.
public sealed class PushQueue : IPushNotifier
{
    private readonly Channel<PushMessage> _channel =
        Channel.CreateUnbounded<PushMessage>(
            new UnboundedChannelOptions { SingleReader = true });

    public Task SendToUsersAsync(
        IReadOnlyCollection<Guid> userIds,
        string message,
        CancellationToken cancellationToken = default)
    {
        // Unbounded kanal — TryWrite uspeva uvek dok Writer nije zatvoren.
        _channel.Writer.TryWrite(new PushMessage(userIds, message));
        return Task.CompletedTask;
    }

    public IAsyncEnumerable<PushMessage> ReadAllAsync(CancellationToken cancellationToken)
        => _channel.Reader.ReadAllAsync(cancellationToken);
}

public sealed record PushMessage(IReadOnlyCollection<Guid> UserIds, string Message);
