using NunyFoodWebApi.Domain.Enums;

namespace NunyFoodWebApi.Application.DTOs.Orders;

public record OrderStatusHistoryDto(
    Guid Id,
    Guid OrderId,
    OrderStatus Status,
    DateTime ChangedAt,
    DateTime CreatedAt);
