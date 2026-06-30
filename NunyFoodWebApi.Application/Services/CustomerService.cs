using AutoMapper;
using NunyFoodWebApi.Application.DTOs.Customers;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Services;

public class CustomerService(
    IRepository<Customer> repository,
    IPasswordHasher passwordHasher,
    IMapper mapper) : ICustomerService
{
    public async Task<CustomerDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var c = await repository.GetByIdAsync(id, ct);
        return c is null ? null : mapper.Map<CustomerDto>(c);
    }

    public async Task<IEnumerable<CustomerDto>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await repository.GetAllAsync(ct);
        return mapper.Map<IEnumerable<CustomerDto>>(list);
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto, CancellationToken ct = default)
    {
        var existing = await repository.FirstOrDefaultAsync(c => c.Email == dto.Email, ct);
        if (existing is not null)
            throw new InvalidOperationException("Un compte avec cet email existe déjà.");

        var customer = new Customer
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            PasswordHash = passwordHasher.Hash(dto.Password)
        };
        repository.Add(customer);
        await repository.SaveChangesAsync(ct);
        return mapper.Map<CustomerDto>(customer);
    }

    public async Task<CustomerDto?> UpdateAsync(Guid id, UpdateCustomerDto dto, CancellationToken ct = default)
    {
        var c = await repository.GetByIdAsync(id, ct);
        if (c is null) return null;

        if (dto.FirstName is not null) c.FirstName = dto.FirstName;
        if (dto.LastName is not null) c.LastName = dto.LastName;
        if (dto.PhoneNumber is not null) c.PhoneNumber = dto.PhoneNumber;

        await repository.SaveChangesAsync(ct);
        return mapper.Map<CustomerDto>(c);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var c = await repository.GetByIdAsync(id, ct);
        if (c is null) return false;

        repository.Remove(c);
        await repository.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> ChangePasswordAsync(Guid id, ChangePasswordDto dto, CancellationToken ct = default)
    {
        var c = await repository.GetByIdAsync(id, ct);
        if (c is null) return false;

        if (!passwordHasher.Verify(dto.CurrentPassword, c.PasswordHash))
            return false;

        c.PasswordHash = passwordHasher.Hash(dto.NewPassword);
        await repository.SaveChangesAsync(ct);
        return true;
    }
}
