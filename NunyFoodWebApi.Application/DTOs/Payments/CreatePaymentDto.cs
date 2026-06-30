using NunyFoodWebApi.Domain.Enums;

namespace NunyFoodWebApi.Application.DTOs.Payments;

public record CreatePaymentDto(Guid OrderId, decimal Amount, PaymentMethod Method);
