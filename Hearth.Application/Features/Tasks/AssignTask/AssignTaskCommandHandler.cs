using Hearth.Application.Common;
using Hearth.Application.Common.Interfaces;
using Hearth.Application.Common.Models;
using Hearth.Application.Common.Notifications.Events;
using MediatR;

namespace Hearth.Application.Features.Tasks.AssignTask;

public sealed class AssignTaskCommandHandler
    : IRequestHandler<AssignTaskCommand, Result<TaskDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IIdentityService _identity;
    private readonly ICurrentUser _currentUser;
    private readonly IPublisher _publisher;

    public AssignTaskCommandHandler(
        IUnitOfWork uow,
        IIdentityService identity,
        ICurrentUser currentUser,
        IPublisher publisher)
    {
        _uow = uow;
        _identity = identity;
        _currentUser = currentUser;
        _publisher = publisher;
    }

    public async Task<Result<TaskDto>> Handle(AssignTaskCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.Id is not { } actorId)
            return Error.Unauthorized("Nedostaje identitet korisnika.", "Auth.NoIdentity");

        // Uloga (Adult) je već sprovedena na endpointu; ovde je scope + provera domaćinstva.
        if (_currentUser.HouseholdId is not { } householdId)
            return Error.Forbidden("Nisi član nijednog domaćinstva.", "Household.NotMember");

        var task = await _uow.Tasks.FirstOrDefaultAsync(
            t => t.Id == request.TaskId && t.HouseholdId == householdId, cancellationToken);

        if (task is null)
            return Error.NotFound("Zadatak nije pronađen.", "Task.NotFound");

        if (request.AssignedToUserId is { } assigneeId)
        {
            var assignee = await _identity.GetUserAsync(assigneeId);
            if (assignee.IsFailure)
                return Error.Validation("Korisnik za dodelu ne postoji.", "Task.AssigneeNotFound");

            if (assignee.Value.HouseholdId != householdId)
                return Error.Validation("Korisnik nije u istom domaćinstvu.", "Task.AssigneeWrongHousehold");
        }

        task.AssignedToUserId = request.AssignedToUserId;

        _uow.Tasks.Update(task);
        await _uow.SaveChangesAsync(cancellationToken);

        // Tek posle commita — ako je (novom) korisniku dodeljeno, obavesti ga lično.
        if (task.AssignedToUserId is { } notifyAssigneeId)
            await _publisher.Publish(
                new TaskAssignedEvent(householdId, actorId, notifyAssigneeId, task.Title), cancellationToken);

        return task.ToDto();
    }
}
