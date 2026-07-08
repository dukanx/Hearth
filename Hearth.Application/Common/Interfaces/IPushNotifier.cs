namespace Hearth.Application.Common.Interfaces;

// Web Push ka svim registrovanim uređajima datih korisnika (kad app nije otvorena).
// Dopuna, ne zamena za IRealtimeNotifier: SignalR pokriva otvorenu aplikaciju.
public interface IPushNotifier
{
    Task SendToUsersAsync(
        IReadOnlyCollection<Guid> userIds,
        string message,
        CancellationToken cancellationToken = default);
}
