namespace Hearth.Application.Common.Interfaces;

// Da li korisnik trenutno ima aktivnu realtime (SignalR) konekciju.
// Koristi se da se web push ne šalje nekome ko je u aplikaciji (dobio je toast).
public interface IRealtimePresence
{
    bool IsOnline(Guid userId);
}
