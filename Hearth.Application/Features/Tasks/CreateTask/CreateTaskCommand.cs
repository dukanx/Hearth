using Hearth.Application.Common;
using Hearth.Application.Common.Models;
using Hearth.Domain.Enums;
using MediatR;

namespace Hearth.Application.Features.Tasks.CreateTask;

public sealed record CreateTaskCommand(
    string Title,
    string? Description,
    TaskPriority Priority,
    DateTime? DueDate,
    Guid? AssignedToUserId) : IRequest<Result<TaskDto>>;
