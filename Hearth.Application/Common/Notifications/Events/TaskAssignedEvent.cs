using MediatR;

namespace Hearth.Application.Common.Notifications.Events;

// Objavljuje se TEK posle uspešnog SaveChanges (dodela kroz zaseban endpoint).
// Ide samo lično dodeljenom korisniku.
public sealed record TaskAssignedEvent(
    Guid HouseholdId,
    Guid ActorId,
    Guid AssigneeId,
    string TaskTitle) : INotification;
