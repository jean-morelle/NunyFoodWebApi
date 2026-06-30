namespace NunyFoodWebApi.Application.DTOs.Auth;

public record TokenDto(string Token, string TokenType, int ExpiresIn);
