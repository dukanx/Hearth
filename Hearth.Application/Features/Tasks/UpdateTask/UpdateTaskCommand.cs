using Hearth.Application.Common;
using Hearth.Application.Common.Models;
using Hearth.Domain.Enums;
using MediatR;

namespace Hearth.Application.Features.Tasks.UpdateTask;

public sealed record UpdateTaskCommand(
    Guid TaskId,
    string Title,
    string? Description,
    TaskPriority Priority,
    DateTime? DueDate) : IRequest<Result<TaskDto>>;
