namespace NunyFoodWebApi.Application.DTOs.Orders;

public record CreateOrderDto(
    Guid CustomerId,
    Guid BeneficiaryId,
    Guid PackId);
