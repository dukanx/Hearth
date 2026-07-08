using Hearth.Api.Extensions;
using Hearth.Application.Features.Shopping.ChangeShoppingItemStatus;
using Hearth.Application.Features.Shopping.CreateShoppingItem;
using Hearth.Application.Features.Shopping.DeleteShoppingItem;
using Hearth.Application.Features.Shopping.GetShoppingItems;
using Hearth.Application.Features.Shopping.UpdateShoppingItem;
using Hearth.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hearth.Api.Controllers;

[ApiController]
[Route("api/shopping")]
[Authorize]
public sealed class ShoppingController : ControllerBase
{
    private readonly ISender _sender;

    public ShoppingController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ShoppingItemStatus? status, CancellationToken ct)
        => (await _sender.Send(new GetShoppingItemsQuery(status), ct)).ToActionResult();

    [HttpPost]
    public async Task<IActionResult> Create(CreateShoppingItemRequest body, CancellationToken ct)
        => (await _sender.Send(new CreateShoppingItemCommand(body.Name, body.Quantity), ct)).ToActionResult();

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateShoppingItemRequest body, CancellationToken ct)
        => (await _sender.Send(new UpdateShoppingItemCommand(id, body.Name, body.Quantity), ct)).ToActionResult();

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, ChangeShoppingItemStatusRequest body, CancellationToken ct)
        => (await _sender.Send(new ChangeShoppingItemStatusCommand(id, body.Status), ct)).ToActionResult();

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => (await _sender.Send(new DeleteShoppingItemCommand(id), ct)).ToActionResult();
}

public sealed record CreateShoppingItemRequest(string Name, int Quantity);

public sealed record UpdateShoppingItemRequest(string Name, int Quantity);

public sealed record ChangeShoppingItemStatusRequest(ShoppingItemStatus Status);
