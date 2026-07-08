using Hearth.Application.Common;
using Hearth.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hearth.Application.Features.Notifications.MarkAllNotificationsRead;

public sealed class MarkAllNotificationsReadCommandHandler : IRequestHandler<MarkAllNotificationsReadCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public MarkAllNotificationsReadCommandHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(MarkAllNotificationsReadCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.Id is not { } userId)
            return Result.Failure(Error.Unauthorized("Nedostaje identitet korisnika.", "Auth.NoIdentity"));

        var unread = await _db.Notifications
            .Where(n => n.RecipientUserId == userId && !n.IsRead)
            .ToListAsync(cancellationToken);

        if (unread.Count == 0)
            return Result.Success();

        foreach (var notification in unread)
            notification.IsRead = true;

        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
