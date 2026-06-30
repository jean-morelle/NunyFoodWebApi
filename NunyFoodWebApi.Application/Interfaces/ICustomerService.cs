using NunyFoodWebApi.Application.DTOs.Customers;


namespace NunyFoodWebApi.Application.Interfaces;

public interface ICustomerService
{
    Task<CustomerDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<CustomerDto>> GetAllAsync(CancellationToken ct = default);
    Task<CustomerDto> CreateAsync(CreateCustomerDto dto, CancellationToken ct = default);
    Task<CustomerDto?> UpdateAsync(Guid id, UpdateCustomerDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<bool> ChangePasswordAsync(Guid id, ChangePasswordDto dto, CancellationToken ct = default);
}
