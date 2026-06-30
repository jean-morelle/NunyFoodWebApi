namespace NunyFoodWebApi.Application.DTOs.Beneficiaries;

public record UpdateBeneficiaryDto(
    string? FullName,
    string? PhoneNumber,
    string? Address,
    string? City);
