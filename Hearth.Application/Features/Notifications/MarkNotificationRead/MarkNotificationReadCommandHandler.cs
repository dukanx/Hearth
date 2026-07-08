using Hearth.Application.Common;
using Hearth.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hearth.Application.Features.Notifications.MarkNotificationRead;

public sealed class MarkNotificationReadCommandHandler : IRequestHandler<MarkNotificationReadCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public MarkNotificationReadCommandHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(MarkNotificationReadCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.Id is not { } userId)
            return Result.Failure(Error.Unauthorized("Nedostaje identitet korisnika.", "Auth.NoIdentity"));

        var notification = await _db.Notifications.FirstOrDefaultAsync(
            n => n.Id == request.NotificationId && n.RecipientUserId == userId, cancellationToken);

        if (notification is null)
            return Result.Failure(Error.NotFound("Obaveštenje nije pronađeno.", "Notification.NotFound"));

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            await _db.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }
}
