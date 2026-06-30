namespace NunyFoodWebApi.Application.DTOs.Customers;

public record CustomerDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    DateTime CreatedAt);
