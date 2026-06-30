namespace NunyFoodWebApi.Application.DTOs.Customers;

public record UpdateCustomerDto(
    string? FirstName,
    string? LastName,
    string? PhoneNumber);
