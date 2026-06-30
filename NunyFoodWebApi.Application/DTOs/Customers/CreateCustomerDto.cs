namespace NunyFoodWebApi.Application.DTOs.Customers;

public record CreateCustomerDto(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Password);
