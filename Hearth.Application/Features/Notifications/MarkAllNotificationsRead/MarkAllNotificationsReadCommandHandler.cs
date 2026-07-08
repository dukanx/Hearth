using Hearth.Application.Common;
using Hearth.Application.Common.Interfaces;
using MediatR;

namespace Hearth.Application.Features.Notifications.MarkAllNotificationsRead;

public sealed class MarkAllNotificationsReadCommandHandler : IRequestHandler<MarkAllNotificationsReadCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _currentUser;

    public MarkAllNotificationsReadCommandHandler(IUnitOfWork uow, ICurrentUser currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(MarkAllNotificationsReadCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.Id is not { } userId)
            return Result.Failure(Error.Unauthorized("Nedostaje identitet korisnika.", "Auth.NoIdentity"));

        var unread = await _uow.Notifications.ListAsync(
            n => n.RecipientUserId == userId && !n.IsRead, cancellationToken);

        if (unread.Count == 0)
            return Result.Success();

        foreach (var notification in unread)
        {
            notification.IsRead = true;
            _uow.Notifications.Update(notification);
        }

        await _uow.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
