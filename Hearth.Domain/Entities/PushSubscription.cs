using Hearth.Domain.Common;

namespace Hearth.Domain.Entities;

// Web Push pretplata jednog browsera/uređaja (korisnik ih može imati više).
public class PushSubscription : BaseEntity
{
    public Guid UserId { get; set; }
    public string Endpoint { get; set; } = string.Empty;
    public string P256dh { get; set; } = string.Empty;
    public string Auth { get; set; } = string.Empty;
}
