using Hearth.Application.Common;
using Hearth.Application.Common.Interfaces;
using Hearth.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hearth.Application.Features.Notifications.GetNotifications;

public sealed class GetNotificationsQueryHandler
    : IRequestHandler<GetNotificationsQuery, Result<IReadOnlyList<NotificationDto>>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public GetNotificationsQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<IReadOnlyList<NotificationDto>>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.Id is not { } userId)
            return Error.Unauthorized("Nedostaje identitet korisnika.", "Auth.NoIdentity");

        // Primalac je uvek trenutni korisnik — obaveštenja su lična.
        var query = _db.Notifications.Where(n => n.RecipientUserId == userId);

        if (request.UnreadOnly)
            query = query.Where(n => !n.IsRead);

        var items = await query
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationDto(n.Id, n.Message, n.IsRead, n.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<NotificationDto>>(items);
    }
}
