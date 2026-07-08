using Hearth.Application.Common.Interfaces;
using Hearth.Domain.Entities;

namespace Hearth.Infrastructure.Persistence.Repositories;

public sealed class ShoppingItemRepository : Repository<ShoppingItem>, IShoppingItemRepository
{
    public ShoppingItemRepository(AppDbContext context) : base(context) { }
}
