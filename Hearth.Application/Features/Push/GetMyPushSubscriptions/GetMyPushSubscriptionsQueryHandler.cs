using Hearth.Application.Common;
using Hearth.Application.Common.Interfaces;
using Hearth.Application.Common.Models;
using MediatR;

namespace Hearth.Application.Features.Push.GetMyPushSubscriptions;

// Dijagnostika za korisnika: koje uređaje backend zna za njegov nalog.
// Ako lista prazna, a browser kaže "Uključeno" — pretplata nije stigla do baze.
public sealed class GetMyPushSubscriptionsQueryHandler
    : IRequestHandler<GetMyPushSubscriptionsQuery, Result<IReadOnlyList<PushSubscriptionDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _currentUser;

    public GetMyPushSubscriptionsQueryHandler(IUnitOfWork uow, ICurrentUser currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<Result<IReadOnlyList<PushSubscriptionDto>>> Handle(
        GetMyPushSubscriptionsQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.Id is not { } userId)
            return Error.Unauthorized("Nedostaje identitet korisnika.", "Auth.NoIdentity");

        var subscriptions = await _uow.PushSubscriptions.ListAsync(
            s => s.UserId == userId, cancellationToken);

        var items = subscriptions
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new PushSubscriptionDto(
                Uri.TryCreate(s.Endpoint, UriKind.Absolute, out var uri) ? uri.Host : "?",
                s.CreatedAt))
            .ToList();

        return Result.Success<IReadOnlyList<PushSubscriptionDto>>(items);
    }
}
