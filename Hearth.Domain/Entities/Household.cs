using Hearth.Domain.Common;

namespace Hearth.Domain.Entities;

public class Household : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    // Dva koda za pridruživanje: jedan dodeljuje ulogu Adult, drugi Child.
    public string AdultJoinCode { get; set; } = string.Empty;
    public string ChildJoinCode { get; set; } = string.Empty;

    public ICollection<HouseholdTask> Tasks { get; set; } = new List<HouseholdTask>();
    public ICollection<ShoppingItem> ShoppingItems { get; set; } = new List<ShoppingItem>();
}
