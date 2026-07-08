using Hearth.Application.Common.Interfaces;
using Hearth.Domain.Entities;

namespace Hearth.Infrastructure.Persistence.Repositories;

public sealed class NotificationRepository : Repository<Notification>, INotificationRepository
{
    public NotificationRepository(AppDbContext context) : base(context) { }
}
