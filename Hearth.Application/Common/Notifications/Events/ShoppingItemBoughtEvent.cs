using MediatR;

namespace Hearth.Application.Common.Notifications.Events;

// Objavljuje se TEK posle uspešnog SaveChanges (samo pri prelasku u Bought).
public sealed record ShoppingItemBoughtEvent(Guid HouseholdId, Guid ActorId, string ItemName) : INotification;
