namespace NunyFoodWebApi.Application.DTOs.Packs;

public record UpdatePackDto(
    string? Name,
    string? Description,
    decimal? Price,
    string? ImageUrl,
    bool? IsActive);
