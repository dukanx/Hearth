using Hearth.Application.Common;
using Hearth.Application.Common.Interfaces;
using Hearth.Application.Common.Models;
using MediatR;

namespace Hearth.Application.Features.Tasks.GetTasks;

public sealed class GetTasksQueryHandler
    : IRequestHandler<GetTasksQuery, Result<IReadOnlyList<TaskDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _currentUser;

    public GetTasksQueryHandler(IUnitOfWork uow, ICurrentUser currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<Result<IReadOnlyList<TaskDto>>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.HouseholdId is not { } householdId)
            return Error.Forbidden("Nisi član nijednog domaćinstva.", "Household.NotMember");

        // Opcioni filteri se sklapaju u jedan predikat (null uslovi se poklope pri prevodu u SQL).
        var tasks = await _uow.Tasks.ListAsync(
            t => t.HouseholdId == householdId
                 && (request.Status == null || t.Status == request.Status)
                 && (request.AssignedToUserId == null || t.AssignedToUserId == request.AssignedToUserId),
            cancellationToken);

        var items = tasks
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => t.ToDto())
            .ToList();

        return Result.Success<IReadOnlyList<TaskDto>>(items);
    }
}
