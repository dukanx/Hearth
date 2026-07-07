using Hearth.Application.Common;
using Hearth.Application.Common.Interfaces;
using Hearth.Application.Common.Models;
using Hearth.Domain.Entities;
using Hearth.Domain.Enums;
using MediatR;

namespace Hearth.Application.Features.Tasks.CreateTask;

public sealed class CreateTaskCommandHandler
    : IRequestHandler<CreateTaskCommand, Result<TaskDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly IIdentityService _identity;
    private readonly ICurrentUser _currentUser;

    public CreateTaskCommandHandler(
        IApplicationDbContext db,
        IIdentityService identity,
        ICurrentUser currentUser)
    {
        _db = db;
        _identity = identity;
        _currentUser = currentUser;
    }

    public async Task<Result<TaskDto>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.Id is not { } userId)
            return Error.Unauthorized("Nedostaje identitet korisnika.", "Auth.NoIdentity");

        if (_currentUser.HouseholdId is not { } householdId)
            return Error.Forbidden("Nisi član nijednog domaćinstva.", "Household.NotMember");

        var assignee = request.AssignedToUserId;
        if (assignee is not null)
        {
            // Samo Adult sme da dodeljuje.
            if (_currentUser.Role != Roles.Adult)
                return Error.Forbidden("Samo Adult može da dodeljuje zadatke.", "Task.AssignForbidden");

            var assigneeCheck = await EnsureAssigneeInHouseholdAsync(assignee.Value, householdId);
            if (assigneeCheck is not null)
                return assigneeCheck;
        }

        var task = new HouseholdTask
        {
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            DueDate = request.DueDate,
            Status = HouseholdTaskStatus.ToDo,
            HouseholdId = householdId,
            CreatedByUserId = userId,
            AssignedToUserId = assignee
        };

        _db.HouseholdTasks.Add(task);
        await _db.SaveChangesAsync(cancellationToken);

        return task.ToDto();
    }

    private async Task<Error?> EnsureAssigneeInHouseholdAsync(Guid assigneeId, Guid householdId)
    {
        var user = await _identity.GetUserAsync(assigneeId);
        if (user.IsFailure)
            return Error.Validation("Korisnik za dodelu ne postoji.", "Task.AssigneeNotFound");

        if (user.Value.HouseholdId != householdId)
            return Error.Validation("Korisnik nije u istom domaćinstvu.", "Task.AssigneeWrongHousehold");

        return null;
    }
}
