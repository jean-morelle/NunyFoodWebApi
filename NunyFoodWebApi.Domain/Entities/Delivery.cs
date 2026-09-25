using NunyFoodWebApi.Domain.Common;

namespace NunyFoodWebApi.Domain.Entities;

public class Delivery : BaseEntity
{
    public Guid OrderId { get; set; }
    public Guid DeliveryAgentId { get; set; }
    public string ReceiverName { get; set; } = string.Empty;
    /// <summary>Chemin privé dans le stockage (IFileStorage), jamais exposé tel quel : servi par l'API après contrôle d'accès.</summary>
    public string? PhotoPath { get; set; }
    public string? SignaturePath { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime? DeliveredAt { get; set; }

    public Order Order { get; set; } = null!;
    public DeliveryAgent DeliveryAgent { get; set; } = null!;
}
