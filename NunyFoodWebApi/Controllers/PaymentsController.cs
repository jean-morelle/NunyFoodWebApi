using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NunyFoodWebApi.Application.DTOs.Payments;
using NunyFoodWebApi.Application.Interfaces;

namespace NunyFoodWebApi.Controllers;

[Authorize(Roles = "Admin,Customer")]
[ApiController]
[Route("api/[controller]")]
public class PaymentsController(
    IPaymentService service,
    IValidator<CreatePaymentDto> createValidator) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var dto = await service.GetByIdAsync(id, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet]
    public async Task<IActionResult> GetByOrderId([FromQuery] Guid orderId, CancellationToken ct) =>
        Ok(await service.GetByOrderIdAsync(orderId, ct));

    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> Create([FromBody] CreatePaymentDto dto, CancellationToken ct)
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
}
