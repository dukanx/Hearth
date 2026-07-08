using Hearth.Api.Extensions;
using Hearth.Application.Features.Push.SubscribeToPush;
using Hearth.Application.Features.Push.UnsubscribeFromPush;
using Hearth.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Hearth.Api.Controllers;

[ApiController]
[Route("api/push")]
[Authorize]
public sealed class PushController : ControllerBase
{
    private readonly ISender _sender;
    private readonly VapidOptions _vapid;

    public PushController(ISender sender, IOptions<VapidOptions> vapid)
    {
        _sender = sender;
        _vapid = vapid.Value;
    }

    // Javni VAPID ključ — frontend ga koristi pri pushManager.subscribe().
    [HttpGet("public-key")]
    public IActionResult PublicKey()
        => string.IsNullOrEmpty(_vapid.PublicKey)
            ? NotFound()
            : Ok(new { publicKey = _vapid.PublicKey });

    [HttpPost("subscribe")]
    public async Task<IActionResult> Subscribe(SubscribeToPushCommand command, CancellationToken ct)
        => (await _sender.Send(command, ct)).ToActionResult();

    [HttpPost("unsubscribe")]
    public async Task<IActionResult> Unsubscribe(UnsubscribeFromPushCommand command, CancellationToken ct)
        => (await _sender.Send(command, ct)).ToActionResult();
}
