using AutoMapper;
using NunyFoodWebApi.Application.DTOs.Beneficiaries;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Services;

public class BeneficiaryService(
    IRepository<Beneficiary> repository,
    IMapper mapper) : IBeneficiaryService
{
    public async Task<BeneficiaryDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var b = await repository.GetByIdAsync(id, ct);
        return b is null ? null : mapper.Map<BeneficiaryDto>(b);
    }

    public async Task<IEnumerable<BeneficiaryDto>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default)
    {
        var list = await repository.FindAsync(b => b.CustomerId == customerId, ct);
        return mapper.Map<IEnumerable<BeneficiaryDto>>(list);
    }

    public async Task<BeneficiaryDto> CreateAsync(CreateBeneficiaryDto dto, CancellationToken ct = default)
    {
        var b = new Beneficiary
        {
            CustomerId = dto.CustomerId,
            FullName = dto.FullName,
            PhoneNumber = dto.PhoneNumber,
            Address = dto.Address,
            City = dto.City
        };
        repository.Add(b);
        await repository.SaveChangesAsync(ct);
        return mapper.Map<BeneficiaryDto>(b);
    }

    public async Task<BeneficiaryDto?> UpdateAsync(Guid id, UpdateBeneficiaryDto dto, CancellationToken ct = default)
    {
        var b = await repository.GetByIdAsync(id, ct);
        if (b is null) return null;

        if (dto.FullName is not null) b.FullName = dto.FullName;
        if (dto.PhoneNumber is not null) b.PhoneNumber = dto.PhoneNumber;
        if (dto.Address is not null) b.Address = dto.Address;
        if (dto.City is not null) b.City = dto.City;

        await repository.SaveChangesAsync(ct);
        return mapper.Map<BeneficiaryDto>(b);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var b = await repository.GetByIdAsync(id, ct);
        if (b is null) return false;

        repository.Remove(b);
        await repository.SaveChangesAsync(ct);
        return true;
    }
}
