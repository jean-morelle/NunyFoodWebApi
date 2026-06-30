using NunyFoodWebApi.Domain.Enums;

namespace NunyFoodWebApi.Application.DTOs.Payments;

public record PaymentDto(
    Guid Id,
    Guid OrderId,
    decimal Amount,
    PaymentMethod Method,
    PaymentStatus Status,
    string? TransactionId,
    DateTime CreatedAt);
