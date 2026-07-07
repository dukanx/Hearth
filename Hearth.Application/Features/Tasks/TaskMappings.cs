using Hearth.Application.Common.Models;
using Hearth.Domain.Entities;

namespace Hearth.Application.Features.Tasks;

internal static class TaskMappings
{
    public static TaskDto ToDto(this HouseholdTask task) => new(
        task.Id,
        task.Title,
        task.Description,
        task.Status,
        task.Priority,
        task.DueDate,
        task.AssignedToUserId,
        task.CreatedByUserId,
        task.CreatedAt);
}
