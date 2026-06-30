namespace NunyFoodWebApi.Application.DTOs.Products;

public record ProductDto(
    Guid Id,
    string Name,
    string Description,
    bool IsActive,
    DateTime CreatedAt);
