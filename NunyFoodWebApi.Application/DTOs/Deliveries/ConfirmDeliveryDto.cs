namespace NunyFoodWebApi.Application.DTOs.Deliveries;

public record ConfirmDeliveryDto(
    string ReceiverName,
    string? PhotoUrl,
    string? SignatureUrl,
    double? Latitude,
    double? Longitude);
