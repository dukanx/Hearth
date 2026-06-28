using Hearth.Domain.Common;
using Hearth.Domain.Enums;

namespace Hearth.Domain.Entities;

public class ShoppingItem : BaseEntity
{
	public string Name { get; set; } = string.Empty;
	public int Quantity { get; set; } = 1;
	public ShoppingItemStatus Status { get; set; } = ShoppingItemStatus.Needed;

	public Guid HouseholdId { get; set; }
	public Household Household { get; set; } = null!;

	public Guid RequestedByUserId { get; set; }
	public Guid? BoughtByUserId { get; set; }
	public DateTime? BoughtAt { get; set; }
}