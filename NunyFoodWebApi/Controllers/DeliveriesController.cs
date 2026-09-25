using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NunyFoodWebApi.Application.Common;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.Features.Deliveries.Commands;
using NunyFoodWebApi.Application.Features.Deliveries.Queries;

namespace NunyFoodWebApi.Controllers;

[Authorize(Roles = "Admin,DeliveryAgent")]
[ApiController]
[Route("api/[controller]")]
public class DeliveriesController(ISender sender) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var dto = await sender.Send(new GetDeliveryByIdQuery(id), ct);
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
            var dto = await sender.Send(new GetDeliveryByOrderQuery(orderId.Value), ct);
            return dto is null ? NotFound() : Ok(dto);
        }

        if (agentId.HasValue)
            return Ok(await sender.Send(new GetDeliveriesByAgentQuery(agentId.Value), ct));

        return BadRequest("Fournir orderId ou agentId.");
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateDeliveryCommand command, CancellationToken ct)
    {
        var created = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPatch("{id:guid}/confirm")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(12 * 1024 * 1024)]
    public async Task<IActionResult> Confirm(Guid id, [FromForm] ConfirmDeliveryForm form, CancellationToken ct)
    {
        if (!TryParseCoordinate(form.Latitude, out var latitude))
            ModelState.AddModelError(nameof(form.Latitude), "Latitude invalide.");
        if (!TryParseCoordinate(form.Longitude, out var longitude))
            ModelState.AddModelError(nameof(form.Longitude), "Longitude invalide.");
        if (!ModelState.IsValid)
            return ValidationProblem();

        var command = new ConfirmDeliveryCommand(
            id, form.ReceiverName ?? string.Empty, latitude, longitude,
            ToUpload(form.Photo), ToUpload(form.Signature));

        var confirmed = await sender.Send(command, ct);
        return confirmed is null ? NotFound() : Ok(confirmed);
    }

    private static FileUpload? ToUpload(IFormFile? file) =>
        file is null ? null : new FileUpload(file.OpenReadStream(), file.ContentType, file.Length);

    // Les coordonnées arrivent en texte ("6.13") : on les lit en culture invariante,
    // indépendamment de la langue du serveur.
    private static bool TryParseCoordinate(string? value, out double? result)
    {
        result = null;
        if (string.IsNullOrWhiteSpace(value)) return true;
        if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed)) return false;
        result = parsed;
        return true;
    }
}

public class ConfirmDeliveryForm
{
    public string? ReceiverName { get; set; }
    public string? Latitude { get; set; }
    public string? Longitude { get; set; }
    public IFormFile? Photo { get; set; }
    public IFormFile? Signature { get; set; }
}
