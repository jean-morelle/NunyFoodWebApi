namespace NunyFoodWebApi.Application.DTOs.Admins;

public record AdminDto(Guid Id, string FullName, string Email, DateTime CreatedAt);
