using Hearth.Domain.Common;

namespace Hearth.Domain.Entities;

public class Notification : BaseEntity
{
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;

    public Guid HouseholdId { get; set; }
    public Household Household { get; set; } = null!;

    public Guid RecipientUserId { get; set; }
}