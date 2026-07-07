using Hearth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hearth.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Household> Households { get; }
    DbSet<HouseholdTask> HouseholdTasks { get; }
    DbSet<ShoppingItem> ShoppingItems { get; }
    DbSet<Notification> Notifications { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
