namespace NunyFoodWebApi.Application.DTOs.DeliveryAgents;

public record UpdateDeliveryAgentDto(
    string? FullName,
    string? PhoneNumber,
    string? Zone,
    bool? IsActive);
