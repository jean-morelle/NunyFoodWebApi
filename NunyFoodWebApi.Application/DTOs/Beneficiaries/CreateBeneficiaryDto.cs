namespace NunyFoodWebApi.Application.DTOs.Beneficiaries;

public record CreateBeneficiaryDto(
    Guid CustomerId,
    string FullName,
    string PhoneNumber,
    string Address,
    string City);
