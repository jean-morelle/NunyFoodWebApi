namespace NunyFoodWebApi.Application.DTOs.Deliveries;

public record CreateDeliveryDto(
    Guid OrderId,
    Guid DeliveryAgentId,
    string ReceiverName);
