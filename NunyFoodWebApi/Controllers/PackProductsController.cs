using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.Features.PackProducts.Commands;
using NunyFoodWebApi.Application.Features.PackProducts.Queries;

namespace NunyFoodWebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/packs/{packId:guid}/products")]
public class PackProductsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(Guid packId, CancellationToken ct) =>
        Ok(await sender.Send(new GetPackProductsQuery(packId), ct));

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Add(Guid packId, [FromBody] AddProductToPackCommand command, CancellationToken ct) =>
        Ok(await sender.Send(command with { PackId = packId }, ct));

    [HttpDelete("{productId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Remove(Guid packId, Guid productId, CancellationToken ct) =>
        await sender.Send(new RemoveProductFromPackCommand(packId, productId), ct) ? NoContent() : NotFound();
}
