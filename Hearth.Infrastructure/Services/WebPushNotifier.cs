using System.Text.Json;
using Hearth.Application.Common.Interfaces;
using Hearth.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebPush;

namespace Hearth.Infrastructure.Services;

public sealed class WebPushNotifier : IPushNotifier
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    // Push servisi (naročito Apple) umeju da "vise" — bez tvrdog timeout-a
    // jedan mrtav endpoint blokira slanje svima ostalima.
    private static readonly HttpClient SharedHttpClient =
        new() { Timeout = TimeSpan.FromSeconds(10) };

    private readonly AppDbContext _db;
    private readonly VapidOptions _vapid;
    private readonly IRealtimePresence _presence;
    private readonly ILogger<WebPushNotifier> _logger;

    public WebPushNotifier(
        AppDbContext db,
        IOptions<VapidOptions> vapid,
        IRealtimePresence presence,
        ILogger<WebPushNotifier> logger)
    {
        _db = db;
        _vapid = vapid.Value;
        _presence = presence;
        _logger = logger;
    }

    public async Task SendToUsersAsync(
        IReadOnlyCollection<Guid> userIds,
        string message,
        CancellationToken cancellationToken = default)
    {
        // Push je opciona nadogradnja — bez ključeva se preskače.
        if (string.IsNullOrEmpty(_vapid.PublicKey) || string.IsNullOrEmpty(_vapid.PrivateKey))
        {
            _logger.LogInformation("Web push preskočen: VAPID ključevi nisu konfigurisani.");
            return;
        }

        // Ko je trenutno u aplikaciji (aktivna SignalR konekcija), već je dobio toast —
        // push ide samo offline korisnicima. Time nema ni dupliranja ni "tihih" push-eva
        // (iOS ukida pretplatu ako service worker ne prikaže notifikaciju).
        var offlineUserIds = userIds.Where(id => !_presence.IsOnline(id)).ToList();

        var subscriptions = await _db.PushSubscriptions
            .Where(s => offlineUserIds.Contains(s.UserId))
            .ToListAsync(cancellationToken);

        _logger.LogInformation(
            "Web push: {SubscriptionCount} pretplata za {OfflineCount}/{RecipientCount} offline primalaca.",
            subscriptions.Count, offlineUserIds.Count, userIds.Count);

        if (subscriptions.Count == 0)
            return;

        var vapidDetails = new VapidDetails(_vapid.Subject, _vapid.PublicKey, _vapid.PrivateKey);
        var payload = JsonSerializer.Serialize(
            new { title = "Hearth", message }, JsonOptions);

        using var client = new WebPushClient(SharedHttpClient);
        var dead = new List<Domain.Entities.PushSubscription>();
        var delivered = 0;

        foreach (var subscription in subscriptions)
        {
            var target = new WebPush.PushSubscription(
                subscription.Endpoint, subscription.P256dh, subscription.Auth);

            try
            {
                await client.SendNotificationAsync(target, payload, vapidDetails, cancellationToken);
                delivered++;
            }
            catch (WebPushException ex) when (
                ex.StatusCode is System.Net.HttpStatusCode.NotFound
                    or System.Net.HttpStatusCode.Gone)
            {
                // Browser je poništio pretplatu — počisti je.
                _logger.LogInformation(
                    "Web push pretplata više ne važi ({StatusCode}), brišem je: {Endpoint}",
                    (int)ex.StatusCode, subscription.Endpoint);
                dead.Add(subscription);
            }
            catch (WebPushException ex)
            {
                // Status + telo odgovora otkrivaju uzrok (npr. Apple 403 BadJwtToken = loš VAPID subject).
                _logger.LogWarning(
                    "Web push odbijen ({StatusCode}) za {Endpoint}: {Reason}",
                    (int)ex.StatusCode, subscription.Endpoint, ex.Message);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning(
                    "Web push timeout (>10s) za {Endpoint} — push servis ne odgovara.",
                    subscription.Endpoint);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Web push nije isporučen ({Endpoint})", subscription.Endpoint);
            }
        }

        _logger.LogInformation(
            "Web push isporučen na {Delivered}/{Total} pretplata.",
            delivered, subscriptions.Count);

        if (dead.Count > 0)
        {
            _db.PushSubscriptions.RemoveRange(dead);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
