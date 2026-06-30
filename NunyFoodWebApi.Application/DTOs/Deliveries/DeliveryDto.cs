namespace NunyFoodWebApi.Application.DTOs.Deliveries;

public record DeliveryDto(
    Guid Id,
    Guid OrderId,
    Guid DeliveryAgentId,
    string ReceiverName,
    string? PhotoUrl,
    string? SignatureUrl,
    double? Latitude,
    double? Longitude,
    DateTime? DeliveredAt,
    DateTime CreatedAt);
