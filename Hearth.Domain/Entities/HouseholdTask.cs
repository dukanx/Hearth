using Hearth.Domain.Common;
using Hearth.Domain.Enums;

namespace Hearth.Domain.Entities;

public class HouseholdTask : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public HouseholdTaskStatus Status { get; set; } = HouseholdTaskStatus.ToDo;
    public DateTime? DueDate { get; set; }
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;


    public Guid HouseholdId { get; set; }
    public Household Household { get; set; } = null!;

    public Guid? AssignedToUserId { get; set; }
    public Guid CreatedByUserId { get; set; }
}