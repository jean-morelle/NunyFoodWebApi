namespace NunyFoodWebApi.Application.DTOs.Products;

public record UpdateProductDto(
    string? Name,
    string? Description,
    bool? IsActive);
