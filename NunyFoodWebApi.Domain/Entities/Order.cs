using NunyFoodWebApi.Domain.Common;
using NunyFoodWebApi.Domain.Enums;

namespace NunyFoodWebApi.Domain.Entities;

public class Order : BaseEntity
{
    public Guid CustomerId { get; set; }
    public Guid BeneficiaryId { get; set; }
    public Guid PackId { get; set; }
    public decimal Amount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Created;

    public Customer Customer { get; set; } = null!;
    public Beneficiary Beneficiary { get; set; } = null!;
    public Pack Pack { get; set; } = null!;
    public ICollection<Payment> Payments { get; set; } = [];
    public ICollection<OrderStatusHistory> StatusHistory { get; set; } = [];
    public Delivery? Delivery { get; set; }
}
