using NunyFoodWebApi.Domain.Common;

namespace NunyFoodWebApi.Domain.Entities;

public class Pack : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<PackProduct> PackProducts { get; set; } = [];
    public ICollection<Order> Orders { get; set; } = [];
}
