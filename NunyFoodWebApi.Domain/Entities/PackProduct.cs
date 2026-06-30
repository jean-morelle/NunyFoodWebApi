namespace NunyFoodWebApi.Domain.Entities;

public class PackProduct
{
    public Guid PackId { get; set; }
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }

    public Pack Pack { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
