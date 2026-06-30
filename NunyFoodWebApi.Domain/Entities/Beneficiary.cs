using NunyFoodWebApi.Domain.Common;

namespace NunyFoodWebApi.Domain.Entities;

public class Beneficiary : BaseEntity
{
    public Guid CustomerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;

    public Customer Customer { get; set; } = null!;
    public ICollection<Order> Orders { get; set; } = [];
}
