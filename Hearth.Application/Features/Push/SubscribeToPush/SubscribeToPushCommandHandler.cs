using Hearth.Application.Common;
using Hearth.Application.Common.Interfaces;
using Hearth.Domain.Entities;
using MediatR;

namespace Hearth.Application.Features.Push.SubscribeToPush;

public sealed class SubscribeToPushCommandHandler : IRequestHandler<SubscribeToPushCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _currentUser;

    public SubscribeToPushCommandHandler(IUnitOfWork uow, ICurrentUser currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(SubscribeToPushCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.Id is not { } userId)
            return Result.Failure(Error.Unauthorized("Nedostaje identitet korisnika.", "Auth.NoIdentity"));

        // Idempotentno: isti endpoint se prepiše (npr. novi login na istom browseru).
        var existing = await _uow.PushSubscriptions.FirstOrDefaultAsync(
            s => s.Endpoint == request.Endpoint, cancellationToken);

        if (existing is not null)
        {
            existing.UserId = userId;
            existing.P256dh = request.P256dh;
            existing.Auth = request.Auth;
            _uow.PushSubscriptions.Update(existing);
        }
        else
        {
            _uow.PushSubscriptions.Add(new PushSubscription
            {
                UserId = userId,
                Endpoint = request.Endpoint,
                P256dh = request.P256dh,
                Auth = request.Auth
            });
        }

        await _uow.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
