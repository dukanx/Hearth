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

    private readonly AppDbContext _db;
    private readonly VapidOptions _vapid;
    private readonly ILogger<WebPushNotifier> _logger;

    public WebPushNotifier(
        AppDbContext db,
        IOptions<VapidOptions> vapid,
        ILogger<WebPushNotifier> logger)
    {
        _db = db;
        _vapid = vapid.Value;
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

        var subscriptions = await _db.PushSubscriptions
            .Where(s => userIds.Contains(s.UserId))
            .ToListAsync(cancellationToken);

        _logger.LogInformation(
            "Web push: {SubscriptionCount} pretplata za {RecipientCount} primalaca.",
            subscriptions.Count, userIds.Count);

        if (subscriptions.Count == 0)
            return;

        var vapidDetails = new VapidDetails(_vapid.Subject, _vapid.PublicKey, _vapid.PrivateKey);
        var payload = JsonSerializer.Serialize(
            new { title = "Hearth", message }, JsonOptions);

        using var client = new WebPushClient();
        var dead = new List<Domain.Entities.PushSubscription>();

        foreach (var subscription in subscriptions)
        {
            var target = new WebPush.PushSubscription(
                subscription.Endpoint, subscription.P256dh, subscription.Auth);

            try
            {
                await client.SendNotificationAsync(target, payload, vapidDetails, cancellationToken);
            }
            catch (WebPushException ex) when (
                ex.StatusCode is System.Net.HttpStatusCode.NotFound
                    or System.Net.HttpStatusCode.Gone)
            {
                // Browser je poništio pretplatu — počisti je.
                dead.Add(subscription);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Web push nije isporučen ({Endpoint})", subscription.Endpoint);
            }
        }

        if (dead.Count > 0)
        {
            _db.PushSubscriptions.RemoveRange(dead);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
