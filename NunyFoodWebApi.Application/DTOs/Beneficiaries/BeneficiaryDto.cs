namespace NunyFoodWebApi.Application.DTOs.Beneficiaries;

public record BeneficiaryDto(
    Guid Id,
    Guid CustomerId,
    string FullName,
    string PhoneNumber,
    string Address,
    string City,
    DateTime CreatedAt);
