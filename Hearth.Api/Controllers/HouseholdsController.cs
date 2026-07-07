using Hearth.Api.Extensions;
using Hearth.Application.Features.Households.CreateHousehold;
using Hearth.Application.Features.Households.JoinHousehold;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hearth.Api.Controllers;

[ApiController]
[Route("api/households")]
[Authorize]
public sealed class HouseholdsController : ControllerBase
{
    private readonly ISender _sender;

    public HouseholdsController(ISender sender) => _sender = sender;

    [HttpPost]
    public async Task<IActionResult> Create(CreateHouseholdCommand command, CancellationToken ct)
        => (await _sender.Send(command, ct)).ToActionResult();

    [HttpPost("join")]
    public async Task<IActionResult> Join(JoinHouseholdCommand command, CancellationToken ct)
        => (await _sender.Send(command, ct)).ToActionResult();
}
