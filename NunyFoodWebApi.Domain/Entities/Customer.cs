using NunyFoodWebApi.Domain.Common;

namespace NunyFoodWebApi.Domain.Entities;

public class Customer : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public ICollection<Beneficiary> Beneficiaries { get; set; } = [];
    public ICollection<Order> Orders { get; set; } = [];
}
