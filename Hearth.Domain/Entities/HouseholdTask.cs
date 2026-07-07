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

    // State machine prelazi (strukturno pravilo domena):
    //   ToDo        -> InProgress | Done   (Done dozvoljen za brze taskove)
    //   InProgress  -> ToDo | Done
    //   Done        -> InProgress          (reopen)
    public bool CanTransitionTo(HouseholdTaskStatus target) => Status switch
    {
        HouseholdTaskStatus.ToDo => target is HouseholdTaskStatus.InProgress or HouseholdTaskStatus.Done,
        HouseholdTaskStatus.InProgress => target is HouseholdTaskStatus.ToDo or HouseholdTaskStatus.Done,
        HouseholdTaskStatus.Done => target is HouseholdTaskStatus.InProgress,
        _ => false
    };
}