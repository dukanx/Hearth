using Hearth.Application.Common.Interfaces;

namespace Hearth.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(
        AppDbContext context,
        ITaskRepository tasks,
        IShoppingItemRepository shoppingItems,
        INotificationRepository notifications,
        IHouseholdRepository households,
        IPushSubscriptionRepository pushSubscriptions)
    {
        _context = context;
        Tasks = tasks;
        ShoppingItems = shoppingItems;
        Notifications = notifications;
        Households = households;
        PushSubscriptions = pushSubscriptions;
    }

    public ITaskRepository Tasks { get; }
    public IShoppingItemRepository ShoppingItems { get; }
    public INotificationRepository Notifications { get; }
    public IHouseholdRepository Households { get; }
    public IPushSubscriptionRepository PushSubscriptions { get; }

    // Jedna transakciona granica — AppDbContext.SaveChangesAsync radi i auditing (CreatedAt).
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}
