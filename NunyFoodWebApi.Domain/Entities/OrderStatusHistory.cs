using NunyFoodWebApi.Domain.Common;
using NunyFoodWebApi.Domain.Enums;

namespace NunyFoodWebApi.Domain.Entities;

public class OrderStatusHistory : BaseEntity
{
    public Guid OrderId { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime ChangedAt { get; set; }

    public Order Order { get; set; } = null!;
}
