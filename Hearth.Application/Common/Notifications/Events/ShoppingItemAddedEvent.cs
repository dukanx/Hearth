using MediatR;

namespace Hearth.Application.Common.Notifications.Events;

// Objavljuje se TEK posle uspešnog SaveChanges.
public sealed record ShoppingItemAddedEvent(Guid HouseholdId, Guid ActorId, string ItemName) : INotification;
