using Hearth.Application.Common;
using Hearth.Application.Common.Interfaces;
using MediatR;

namespace Hearth.Application.Features.Notifications.MarkNotificationRead;

public sealed class MarkNotificationReadCommandHandler : IRequestHandler<MarkNotificationReadCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _currentUser;

    public MarkNotificationReadCommandHandler(IUnitOfWork uow, ICurrentUser currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(MarkNotificationReadCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.Id is not { } userId)
            return Result.Failure(Error.Unauthorized("Nedostaje identitet korisnika.", "Auth.NoIdentity"));

        var notification = await _uow.Notifications.FirstOrDefaultAsync(
            n => n.Id == request.NotificationId && n.RecipientUserId == userId, cancellationToken);

        if (notification is null)
            return Result.Failure(Error.NotFound("Obaveštenje nije pronađeno.", "Notification.NotFound"));

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            _uow.Notifications.Update(notification);
            await _uow.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }
}
