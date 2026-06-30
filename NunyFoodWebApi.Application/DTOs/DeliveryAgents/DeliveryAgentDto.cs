namespace NunyFoodWebApi.Application.DTOs.DeliveryAgents;

public record DeliveryAgentDto(
    Guid Id,
    string FullName,
    string Email,
    string PhoneNumber,
    string Zone,
    bool IsActive,
    DateTime CreatedAt);
