using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.Features.Customers.Commands;
using NunyFoodWebApi.Application.Features.Customers.Queries;

namespace NunyFoodWebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CustomersController(ISender sender) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll(CancellationToken ct) =>
        Ok(await sender.Send(new GetCustomersQuery(), ct));

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin,Customer")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var dto = await sender.Send(new GetCustomerByIdQuery(id), ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerCommand command, CancellationToken ct)
    {
        var created = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Customer")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerCommand command, CancellationToken ct)
    {
        var updated = await sender.Send(command with { Id = id }, ct);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) =>
        await sender.Send(new DeleteCustomerCommand(id), ct) ? NoContent() : NotFound();

    [HttpPatch("{id:guid}/change-password")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordCommand command, CancellationToken ct) =>
        await sender.Send(command with { CustomerId = id }, ct) ? NoContent() : NotFound();
}
