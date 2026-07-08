using MediatR;

namespace Hearth.Application.Common.Notifications.Events;

// Objavljuje se TEK posle uspešnog SaveChanges.
// AssignedToUserId != null znači da je zadatak odmah dodeljen — taj korisnik
// dobija ličnu poruku, ostali (sem aktera i dodeljenog) opštu "nov zadatak".
public sealed record TaskCreatedEvent(
    Guid HouseholdId,
    Guid ActorId,
    Guid? AssignedToUserId,
    string TaskTitle) : INotification;
