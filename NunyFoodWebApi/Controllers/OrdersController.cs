using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.Features.Orders.Commands;
using NunyFoodWebApi.Application.Features.Orders.Queries;

namespace NunyFoodWebApi.Controllers;

[Authorize(Roles = "Admin,Customer")]
[ApiController]
[Route("api/[controller]")]
public class OrdersController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? customerId, CancellationToken ct) =>
        Ok(await sender.Send(new GetOrdersQuery(customerId), ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var dto = await sender.Send(new GetOrderByIdQuery(id), ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> Create([FromBody] CreateOrderCommand command, CancellationToken ct)
    {
        var created = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusCommand command, CancellationToken ct)
    {
        var updated = await sender.Send(command with { Id = id }, ct);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpPost("{id:guid}/cancel")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
    {
        var cancelled = await sender.Send(new CancelOrderCommand(id), ct);
        return cancelled is null ? NotFound() : Ok(cancelled);
    }

    [HttpPost("{id:guid}/confirm-reception")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> ConfirmReception(Guid id, CancellationToken ct)
    {
        var confirmed = await sender.Send(new ConfirmReceptionCommand(id), ct);
        return confirmed is null ? NotFound() : Ok(confirmed);
    }

    [HttpGet("{id:guid}/status-history")]
    public async Task<IActionResult> GetStatusHistory(Guid id, CancellationToken ct)
    {
        var history = await sender.Send(new GetOrderStatusHistoryQuery(id), ct);
        return history is null ? NotFound() : Ok(history);
    }
}
