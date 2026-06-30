using NunyFoodWebApi.Domain.Common;
using NunyFoodWebApi.Domain.Enums;

namespace NunyFoodWebApi.Domain.Entities;

public class Payment : BaseEntity
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? TransactionId { get; set; }

    public Order Order { get; set; } = null!;
}
