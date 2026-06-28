using Hearth.Domain.Common;

namespace Hearth.Domain.Entities;

public class Household : BaseEntity
{
    public string Name { get; set; } = string.Empty;
	public string JoinCode { get; set; } = string.Empty;

	public ICollection<HouseholdTask> Tasks { get; set; } = new List<HouseholdTask>();
    public ICollection<ShoppingItem> ShoppingItems { get; set; } = new List<ShoppingItem>();
	

}