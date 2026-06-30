namespace NunyFoodWebApi.Application.DTOs.Packs;

public record CreatePackDto(
    string Name,
    string Description,
    decimal Price,
    string? ImageUrl);
