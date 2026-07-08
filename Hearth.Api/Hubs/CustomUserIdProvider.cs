using Microsoft.AspNetCore.SignalR;

namespace Hearth.Api.Hubs;

// SignalR podrazumevano čita NameIdentifier; naš userId je u 'sub' claim-u
// (MapInboundClaims=false), pa ga ovde eksplicitno vežemo za Clients.User(...).
public sealed class CustomUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
        => connection.User?.FindFirst("sub")?.Value;
}
