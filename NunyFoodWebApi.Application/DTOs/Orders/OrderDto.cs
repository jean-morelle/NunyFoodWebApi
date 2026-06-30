using NunyFoodWebApi.Domain.Enums;

namespace NunyFoodWebApi.Application.DTOs.Orders;

public record OrderDto(
    Guid Id,
    Guid CustomerId,
    Guid BeneficiaryId,
    Guid PackId,
    decimal Amount,
    OrderStatus Status,
    DateTime CreatedAt);
