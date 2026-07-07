using Hearth.Application.Common;
using Hearth.Application.Common.Models;
using MediatR;

namespace Hearth.Application.Features.Tasks.AssignTask;

// AssignedToUserId == null skida dodelu.
public sealed record AssignTaskCommand(Guid TaskId, Guid? AssignedToUserId)
    : IRequest<Result<TaskDto>>;
