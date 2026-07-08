using Hearth.Application.Common;
using Hearth.Application.Common.Interfaces;
using MediatR;

namespace Hearth.Application.Features.Push.UnsubscribeFromPush;

public sealed class UnsubscribeFromPushCommandHandler
    : IRequestHandler<UnsubscribeFromPushCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _currentUser;

    public UnsubscribeFromPushCommandHandler(IUnitOfWork uow, ICurrentUser currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(UnsubscribeFromPushCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.Id is not { } userId)
            return Result.Failure(Error.Unauthorized("Nedostaje identitet korisnika.", "Auth.NoIdentity"));

        var subscription = await _uow.PushSubscriptions.FirstOrDefaultAsync(
            s => s.Endpoint == request.Endpoint && s.UserId == userId, cancellationToken);

        // Idempotentno — nepostojeća pretplata je već "odjavljena".
        if (subscription is not null)
        {
            _uow.PushSubscriptions.Remove(subscription);
            await _uow.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }
}
