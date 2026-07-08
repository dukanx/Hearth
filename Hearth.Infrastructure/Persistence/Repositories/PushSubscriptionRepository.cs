using Hearth.Application.Common.Interfaces;
using Hearth.Domain.Entities;

namespace Hearth.Infrastructure.Persistence.Repositories;

public sealed class PushSubscriptionRepository
    : Repository<PushSubscription>, IPushSubscriptionRepository
{
    public PushSubscriptionRepository(AppDbContext context) : base(context) { }
}
