namespace Hearth.Application.Common.Interfaces;

// Fasada nad repozitorijumima + jedna transakciona granica (SaveChanges).
// Svi repozitorijumi dele isti DbContext (change-tracker), pa se sve promene
// commit-uju zajedno.
public interface IUnitOfWork
{
    ITaskRepository Tasks { get; }
    IShoppingItemRepository ShoppingItems { get; }
    INotificationRepository Notifications { get; }
    IHouseholdRepository Households { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
