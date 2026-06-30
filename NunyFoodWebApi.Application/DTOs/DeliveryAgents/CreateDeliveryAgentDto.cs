namespace NunyFoodWebApi.Application.DTOs.DeliveryAgents;

public record CreateDeliveryAgentDto(
    string FullName,
    string Email,
    string Password,
    string PhoneNumber,
    string Zone);
