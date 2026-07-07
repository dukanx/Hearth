using Hearth.Application.Common;
using MediatR;

namespace Hearth.Application.Features.Tasks.DeleteTask;

public sealed record DeleteTaskCommand(Guid TaskId) : IRequest<Result>;
