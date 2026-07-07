using Hearth.Domain.Enums;

namespace Hearth.Application.Common.Models;

public sealed record TaskDto(
    Guid Id,
    string Title,
    string? Description,
    HouseholdTaskStatus Status,
    TaskPriority Priority,
    DateTime? DueDate,
    Guid? AssignedToUserId,
    Guid CreatedByUserId,
    DateTime CreatedAt);
