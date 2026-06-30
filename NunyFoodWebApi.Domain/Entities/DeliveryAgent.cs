using NunyFoodWebApi.Domain.Common;

namespace NunyFoodWebApi.Domain.Entities;

public class DeliveryAgent : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<Delivery> Deliveries { get; set; } = [];
}
