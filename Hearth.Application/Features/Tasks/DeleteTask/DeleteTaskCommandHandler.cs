using Hearth.Application.Common;
using Hearth.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hearth.Application.Features.Tasks.DeleteTask;

public sealed class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public DeleteTaskCommandHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.HouseholdId is not { } householdId)
            return Result.Failure(Error.Forbidden("Nisi član nijednog domaćinstva.", "Household.NotMember"));

        var task = await _db.HouseholdTasks.FirstOrDefaultAsync(
            t => t.Id == request.TaskId && t.HouseholdId == householdId, cancellationToken);

        if (task is null)
            return Result.Failure(Error.NotFound("Zadatak nije pronađen.", "Task.NotFound"));

        _db.HouseholdTasks.Remove(task);
        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
