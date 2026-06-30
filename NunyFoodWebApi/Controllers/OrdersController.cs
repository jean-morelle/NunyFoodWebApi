using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NunyFoodWebApi.Application.DTOs.Orders;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Enums;

namespace NunyFoodWebApi.Controllers;

[Authorize(Roles = "Admin,Customer")]
[ApiController]
[Route("api/[controller]")]
public class OrdersController(
    IOrderService service,
    IValidator<CreateOrderDto> createValidator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? customerId, CancellationToken ct)
    {
        if (customerId.HasValue)
            return Ok(await service.GetByCustomerIdAsync(customerId.Value, ct));
        return Ok(await service.GetAllAsync(ct));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var dto = await service.GetByIdAsync(id, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto, CancellationToken ct)
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

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusRequest request, CancellationToken ct)
    {
        var updated = await service.UpdateStatusAsync(id, request.Status, ct);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpGet("{id:guid}/status-history")]
    public async Task<IActionResult> GetStatusHistory(Guid id, CancellationToken ct) =>
        Ok(await service.GetStatusHistoryAsync(id, ct));
}

public record UpdateOrderStatusRequest(OrderStatus Status);
