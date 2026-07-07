using Hearth.Application.Common;
using Hearth.Application.Common.Interfaces;
using Hearth.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hearth.Application.Features.Tasks.GetTaskById;

public sealed class GetTaskByIdQueryHandler
    : IRequestHandler<GetTaskByIdQuery, Result<TaskDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public GetTaskByIdQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<TaskDto>> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.HouseholdId is not { } householdId)
            return Error.Forbidden("Nisi član nijednog domaćinstva.", "Household.NotMember");

        var task = await _db.HouseholdTasks.FirstOrDefaultAsync(
            t => t.Id == request.TaskId && t.HouseholdId == householdId, cancellationToken);

        if (task is null)
            return Error.NotFound("Zadatak nije pronađen.", "Task.NotFound");

        return task.ToDto();
    }
}
