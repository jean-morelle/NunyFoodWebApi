namespace NunyFoodWebApi.Application.DTOs.Packs;

public record PackDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string? ImageUrl,
    bool IsActive,
    DateTime CreatedAt);
