using Hearth.Api.Extensions;
using Hearth.Application.Features.Notifications.GetNotifications;
using Hearth.Application.Features.Notifications.MarkAllNotificationsRead;
using Hearth.Application.Features.Notifications.MarkNotificationRead;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hearth.Api.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public sealed class NotificationsController : ControllerBase
{
    private readonly ISender _sender;

    public NotificationsController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] bool unreadOnly, CancellationToken ct)
        => (await _sender.Send(new GetNotificationsQuery(unreadOnly), ct)).ToActionResult();

    [HttpPut("{id:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken ct)
        => (await _sender.Send(new MarkNotificationReadCommand(id), ct)).ToActionResult();

    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllRead(CancellationToken ct)
        => (await _sender.Send(new MarkAllNotificationsReadCommand(), ct)).ToActionResult();
}
