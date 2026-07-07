using Hearth.Application.Common;
using Hearth.Application.Common.Interfaces;
using Hearth.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hearth.Application.Features.Tasks.ChangeTaskStatus;

public sealed class ChangeTaskStatusCommandHandler
    : IRequestHandler<ChangeTaskStatusCommand, Result<TaskDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public ChangeTaskStatusCommandHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<TaskDto>> Handle(ChangeTaskStatusCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.Id is not { } userId)
            return Error.Unauthorized("Nedostaje identitet korisnika.", "Auth.NoIdentity");

        if (_currentUser.HouseholdId is not { } householdId)
            return Error.Forbidden("Nisi član nijednog domaćinstva.", "Household.NotMember");

        var task = await _db.HouseholdTasks.FirstOrDefaultAsync(
            t => t.Id == request.TaskId && t.HouseholdId == householdId, cancellationToken);

        if (task is null)
            return Error.NotFound("Zadatak nije pronađen.", "Task.NotFound");

        // Status menja Adult (bilo koji task) ili Child kome je task dodeljen.
        var isAdult = _currentUser.Role == Roles.Adult;
        var isAssignee = task.AssignedToUserId == userId;
        if (!isAdult && !isAssignee)
            return Error.Forbidden("Status možeš da menjaš samo na svom dodeljenom zadatku.", "Task.StatusForbidden");

        if (task.Status == request.Status)
            return Error.Validation("Zadatak je već u tom statusu.", "Task.SameStatus");

        if (!task.CanTransitionTo(request.Status))
            return Error.Validation(
                $"Nevažeći prelaz statusa: {task.Status} -> {request.Status}.", "Task.InvalidTransition");

        task.Status = request.Status;
        await _db.SaveChangesAsync(cancellationToken);

        return task.ToDto();
    }
}
