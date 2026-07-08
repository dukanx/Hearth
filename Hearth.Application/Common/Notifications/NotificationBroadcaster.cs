using Hearth.Application.Common.Interfaces;
using Hearth.Application.Common.Models;
using Hearth.Domain.Entities;

namespace Hearth.Application.Common.Notifications;

// Zajednička logika za sve okidače: nađe primaoce, upiše Notification redove
// (istorija + mark-read) i gurne ih uživo. Kompoziciju poruke rade sami handleri.
public sealed class NotificationBroadcaster
{
    private readonly IUnitOfWork _uow;
    private readonly IIdentityService _identity;
    private readonly IRealtimeNotifier _realtime;

    public NotificationBroadcaster(
        IUnitOfWork uow,
        IIdentityService identity,
        IRealtimeNotifier realtime)
    {
        _uow = uow;
        _identity = identity;
        _realtime = realtime;
    }

    // Svi članovi domaćinstva osim navedenih (npr. aktera i, kod zadataka, dodeljenog).
    public async Task BroadcastToHouseholdAsync(
        Guid householdId,
        IEnumerable<Guid> excludedUserIds,
        string message,
        CancellationToken cancellationToken)
    {
        var members = await _identity.GetHouseholdMemberIdsAsync(householdId);
        if (members.IsFailure)
            return;

        var excluded = excludedUserIds.ToHashSet();
        var recipients = members.Value.Where(id => !excluded.Contains(id)).ToList();

        await PersistAndPushAsync(householdId, recipients, message, cancellationToken);
    }

    // Lična poruka jednom korisniku (npr. dodela zadatka).
    public Task SendToUserAsync(
        Guid householdId,
        Guid recipientId,
        string message,
        CancellationToken cancellationToken)
        => PersistAndPushAsync(householdId, new[] { recipientId }, message, cancellationToken);

    private async Task PersistAndPushAsync(
        Guid householdId,
        IReadOnlyCollection<Guid> recipients,
        string message,
        CancellationToken cancellationToken)
    {
        if (recipients.Count == 0)
            return;

        var notifications = recipients
            .Select(recipientId => new Notification
            {
                Message = message,
                HouseholdId = householdId,
                RecipientUserId = recipientId,
                IsRead = false
            })
            .ToList();

        foreach (var notification in notifications)
            _uow.Notifications.Add(notification);

        // CreatedAt postavlja SaveChanges override; Id se generiše ovde.
        await _uow.SaveChangesAsync(cancellationToken);

        foreach (var notification in notifications)
        {
            var dto = new NotificationDto(
                notification.Id,
                notification.Message,
                notification.IsRead,
                notification.CreatedAt);

            await _realtime.SendToUserAsync(notification.RecipientUserId, dto, cancellationToken);
        }
    }
}
