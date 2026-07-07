using Hearth.Application.Common;
using Hearth.Application.Common.Models;
using MediatR;

namespace Hearth.Application.Features.Tasks.GetTaskById;

public sealed record GetTaskByIdQuery(Guid TaskId) : IRequest<Result<TaskDto>>;
