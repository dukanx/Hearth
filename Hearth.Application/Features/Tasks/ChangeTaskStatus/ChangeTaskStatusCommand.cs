using Hearth.Application.Common;
using Hearth.Application.Common.Models;
using Hearth.Domain.Enums;
using MediatR;

namespace Hearth.Application.Features.Tasks.ChangeTaskStatus;

public sealed record ChangeTaskStatusCommand(Guid TaskId, HouseholdTaskStatus Status)
    : IRequest<Result<TaskDto>>;
