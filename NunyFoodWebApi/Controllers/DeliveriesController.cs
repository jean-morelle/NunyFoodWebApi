using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NunyFoodWebApi.Application.DTOs.Deliveries;
using NunyFoodWebApi.Application.Interfaces;

namespace NunyFoodWebApi.Controllers;

[Authorize(Roles = "Admin,DeliveryAgent")]
[ApiController]
[Route("api/[controller]")]
public class DeliveriesController(
    IDeliveryService service,
    IValidator<CreateDeliveryDto> createValidator,
    IValidator<ConfirmDeliveryDto> confirmValidator) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var dto = await service.GetByIdAsync(id, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet]
    public async Task<IActionResult> GetDeliveries(
        [FromQuery] Guid? orderId,
        [FromQuery] Guid? agentId,
        CancellationToken ct)
    {
        if (orderId.HasValue)
        {
            var dto = await service.GetByOrderIdAsync(orderId.Value, ct);
            return dto is null ? NotFound() : Ok(dto);
        }

        if (agentId.HasValue)
            return Ok(await service.GetByAgentIdAsync(agentId.Value, ct));

        return BadRequest("Fournir orderId ou agentId.");
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateDeliveryDto dto, CancellationToken ct)
    {
        var result = await createValidator.ValidateAsync(dto, ct);
        if (!result.IsValid)
        {
            foreach (var e in result.Errors) ModelState.AddModelError(e.PropertyName, e.ErrorMessage);
            return ValidationProblem();
        }
        var created = await service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPatch("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid id, [FromBody] ConfirmDeliveryDto dto, CancellationToken ct)
    {
        var result = await confirmValidator.ValidateAsync(dto, ct);
        if (!result.IsValid)
        {
            foreach (var e in result.Errors) ModelState.AddModelError(e.PropertyName, e.ErrorMessage);
            return ValidationProblem();
        }
        var confirmed = await service.ConfirmAsync(id, dto, ct);
        return confirmed is null ? NotFound() : Ok(confirmed);
    }
}
