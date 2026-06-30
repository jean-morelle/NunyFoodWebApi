using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NunyFoodWebApi.Application.DTOs.PackProducts;
using NunyFoodWebApi.Application.Interfaces;

namespace NunyFoodWebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/packs/{packId:guid}/products")]
public class PackProductsController(
    IPackProductService service,
    IValidator<AddProductToPackDto> validator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(Guid packId, CancellationToken ct) =>
        Ok(await service.GetByPackIdAsync(packId, ct));

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Add(Guid packId, [FromBody] AddProductToPackDto dto, CancellationToken ct)
    {
        var result = await validator.ValidateAsync(dto, ct);
        if (!result.IsValid)
        {
            foreach (var e in result.Errors) ModelState.AddModelError(e.PropertyName, e.ErrorMessage);
            return ValidationProblem();
        }
        return Ok(await service.AddAsync(packId, dto, ct));
    }

    [HttpDelete("{productId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Remove(Guid packId, Guid productId, CancellationToken ct)
    {
        var removed = await service.RemoveAsync(packId, productId, ct);
        return removed ? NoContent() : NotFound();
    }
}
