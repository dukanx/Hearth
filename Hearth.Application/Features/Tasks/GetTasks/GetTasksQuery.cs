using Hearth.Application.Common;
using Hearth.Application.Common.Models;
using Hearth.Domain.Enums;
using MediatR;

namespace Hearth.Application.Features.Tasks.GetTasks;

public sealed record GetTasksQuery(HouseholdTaskStatus? Status, Guid? AssignedToUserId)
    : IRequest<Result<IReadOnlyList<TaskDto>>>;
