using Hearth.Application.Common;
using Hearth.Application.Common.Interfaces;
using Hearth.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hearth.Application.Features.Tasks.GetTasks;

public sealed class GetTasksQueryHandler
    : IRequestHandler<GetTasksQuery, Result<IReadOnlyList<TaskDto>>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public GetTasksQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<IReadOnlyList<TaskDto>>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.HouseholdId is not { } householdId)
            return Error.Forbidden("Nisi član nijednog domaćinstva.", "Household.NotMember");

        var query = _db.HouseholdTasks.Where(t => t.HouseholdId == householdId);

        if (request.Status is { } status)
            query = query.Where(t => t.Status == status);

        if (request.AssignedToUserId is { } assignee)
            query = query.Where(t => t.AssignedToUserId == assignee);

        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new TaskDto(
                t.Id,
                t.Title,
                t.Description,
                t.Status,
                t.Priority,
                t.DueDate,
                t.AssignedToUserId,
                t.CreatedByUserId,
                t.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<TaskDto>>(items);
    }
}
