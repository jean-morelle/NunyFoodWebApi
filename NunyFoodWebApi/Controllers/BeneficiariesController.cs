using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.Features.Beneficiaries.Commands;
using NunyFoodWebApi.Application.Features.Beneficiaries.Queries;

namespace NunyFoodWebApi.Controllers;

[Authorize(Roles = "Admin,Customer")]
[ApiController]
[Route("api/[controller]")]
public class BeneficiariesController(ISender sender) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var dto = await sender.Send(new GetBeneficiaryByIdQuery(id), ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet]
    public async Task<IActionResult> GetByCustomerId([FromQuery] Guid customerId, CancellationToken ct) =>
        Ok(await sender.Send(new GetBeneficiariesByCustomerQuery(customerId), ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBeneficiaryCommand command, CancellationToken ct)
    {
        var created = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBeneficiaryCommand command, CancellationToken ct)
    {
        var updated = await sender.Send(command with { Id = id }, ct);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) =>
        await sender.Send(new DeleteBeneficiaryCommand(id), ct) ? NoContent() : NotFound();
}
