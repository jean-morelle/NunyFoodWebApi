namespace NunyFoodWebApi.Application.DTOs.Auth;

public record OtpChallengeDto(string Email, int ExpiresIn);
