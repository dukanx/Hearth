using Hearth.Api.Extensions;
using Hearth.Application.Common;
using Hearth.Application.Features.Tasks.AssignTask;
using Hearth.Application.Features.Tasks.ChangeTaskStatus;
using Hearth.Application.Features.Tasks.CreateTask;
using Hearth.Application.Features.Tasks.DeleteTask;
using Hearth.Application.Features.Tasks.GetTaskById;
using Hearth.Application.Features.Tasks.GetTasks;
using Hearth.Application.Features.Tasks.UpdateTask;
using Hearth.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hearth.Api.Controllers;

[ApiController]
[Route("api/tasks")]
[Authorize]
public sealed class TasksController : ControllerBase
{
    private readonly ISender _sender;

    public TasksController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] HouseholdTaskStatus? status,
        [FromQuery] Guid? assignedToUserId,
        CancellationToken ct)
        => (await _sender.Send(new GetTasksQuery(status, assignedToUserId), ct)).ToActionResult();

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => (await _sender.Send(new GetTaskByIdQuery(id), ct)).ToActionResult();

    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskRequest body, CancellationToken ct)
        => (await _sender.Send(
                new CreateTaskCommand(body.Title, body.Description, body.Priority, body.DueDate, body.AssignedToUserId),
                ct))
            .ToActionResult();

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Adult)]
    public async Task<IActionResult> Update(Guid id, UpdateTaskRequest body, CancellationToken ct)
        => (await _sender.Send(
                new UpdateTaskCommand(id, body.Title, body.Description, body.Priority, body.DueDate),
                ct))
            .ToActionResult();

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, ChangeStatusRequest body, CancellationToken ct)
        => (await _sender.Send(new ChangeTaskStatusCommand(id, body.Status), ct)).ToActionResult();

    [HttpPut("{id:guid}/assign")]
    [Authorize(Roles = Roles.Adult)]
    public async Task<IActionResult> Assign(Guid id, AssignTaskRequest body, CancellationToken ct)
        => (await _sender.Send(new AssignTaskCommand(id, body.AssignedToUserId), ct)).ToActionResult();

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Adult)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => (await _sender.Send(new DeleteTaskCommand(id), ct)).ToActionResult();
}

public sealed record CreateTaskRequest(
    string Title,
    string? Description,
    TaskPriority Priority,
    DateTime? DueDate,
    Guid? AssignedToUserId);

public sealed record UpdateTaskRequest(
    string Title,
    string? Description,
    TaskPriority Priority,
    DateTime? DueDate);

public sealed record ChangeStatusRequest(HouseholdTaskStatus Status);

public sealed record AssignTaskRequest(Guid? AssignedToUserId);
