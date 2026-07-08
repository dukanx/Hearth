using Hearth.Application.Common;
using Hearth.Application.Common.Interfaces;
using MediatR;

namespace Hearth.Application.Features.Tasks.DeleteTask;

public sealed class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _currentUser;

    public DeleteTaskCommandHandler(IUnitOfWork uow, ICurrentUser currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.HouseholdId is not { } householdId)
            return Result.Failure(Error.Forbidden("Nisi član nijednog domaćinstva.", "Household.NotMember"));

        var task = await _uow.Tasks.FirstOrDefaultAsync(
            t => t.Id == request.TaskId && t.HouseholdId == householdId, cancellationToken);

        if (task is null)
            return Result.Failure(Error.NotFound("Zadatak nije pronađen.", "Task.NotFound"));

        _uow.Tasks.Remove(task);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
