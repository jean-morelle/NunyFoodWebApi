using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NunyFoodWebApi.Application.Common;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.Features.Deliveries.Commands;
using NunyFoodWebApi.Application.Features.Deliveries.Queries;

namespace NunyFoodWebApi.Controllers;

// Rôles définis action par action : un [Authorize(Roles)] de classe s'ajouterait à ceux des actions
// et bloquerait les clients sur les routes de consultation.
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DeliveriesController(ISender sender) : ControllerBase
{
    private const string Viewers = "Admin,DeliveryAgent,Customer";

    [HttpGet("{id:guid}")]
    [Authorize(Roles = Viewers)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var dto = await sender.Send(new GetDeliveryByIdQuery(id), ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet]
    [Authorize(Roles = Viewers)]
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

        // Un client consulte la livraison d'une de ses commandes, jamais la liste d'un livreur.
        if (agentId.HasValue && !User.IsInRole("Customer"))
            return Ok(await sender.Send(new GetDeliveriesByAgentQuery(agentId.Value), ct));

        return BadRequest("Fournir orderId ou agentId.");
    }

    [HttpGet("{id:guid}/photo")]
    [Authorize(Roles = Viewers)]
    public Task<IActionResult> GetPhoto(Guid id, CancellationToken ct) => Proof(id, DeliveryProofKind.Photo, ct);

    [HttpGet("{id:guid}/signature")]
    [Authorize(Roles = Viewers)]
    public Task<IActionResult> GetSignature(Guid id, CancellationToken ct) => Proof(id, DeliveryProofKind.Signature, ct);

    private async Task<IActionResult> Proof(Guid id, DeliveryProofKind kind, CancellationToken ct)
    {
        var file = await sender.Send(new GetDeliveryProofQuery(id, kind), ct);
        if (file is null) return NotFound();

        // Donnée personnelle : pas de cache partagé (proxy, CDN), et le navigateur respecte le type annoncé.
        Response.Headers.CacheControl = "private, max-age=3600";
        Response.Headers.XContentTypeOptions = "nosniff";
        return File(file.Content, file.ContentType);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateDeliveryCommand command, CancellationToken ct)
    {
        var created = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPatch("{id:guid}/confirm")]
    [Authorize(Roles = "Admin,DeliveryAgent")]
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
