namespace NunyFoodWebApi.Application.DTOs.PackProducts;

public record PackProductDto(Guid PackId, Guid ProductId, decimal Quantity);
