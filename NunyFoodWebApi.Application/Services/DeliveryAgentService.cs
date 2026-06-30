using AutoMapper;
using NunyFoodWebApi.Application.DTOs.DeliveryAgents;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Services;

public class DeliveryAgentService(
    IRepository<DeliveryAgent> repository,
    IPasswordHasher passwordHasher,
    IMapper mapper) : IDeliveryAgentService
{
    public async Task<DeliveryAgentDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var a = await repository.GetByIdAsync(id, ct);
        return a is null ? null : mapper.Map<DeliveryAgentDto>(a);
    }

    public async Task<IEnumerable<DeliveryAgentDto>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await repository.GetAllAsync(ct);
        return mapper.Map<IEnumerable<DeliveryAgentDto>>(list);
    }

    public async Task<DeliveryAgentDto> CreateAsync(CreateDeliveryAgentDto dto, CancellationToken ct = default)
    {
        var a = new DeliveryAgent
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = passwordHasher.Hash(dto.Password),
            PhoneNumber = dto.PhoneNumber,
            Zone = dto.Zone,
            IsActive = true
        };
        repository.Add(a);
        await repository.SaveChangesAsync(ct);
        return mapper.Map<DeliveryAgentDto>(a);
    }

    public async Task<DeliveryAgentDto?> UpdateAsync(Guid id, UpdateDeliveryAgentDto dto, CancellationToken ct = default)
    {
        var a = await repository.GetByIdAsync(id, ct);
        if (a is null) return null;

        if (dto.FullName is not null) a.FullName = dto.FullName;
        if (dto.PhoneNumber is not null) a.PhoneNumber = dto.PhoneNumber;
        if (dto.Zone is not null) a.Zone = dto.Zone;
        if (dto.IsActive is not null) a.IsActive = dto.IsActive.Value;

        await repository.SaveChangesAsync(ct);
        return mapper.Map<DeliveryAgentDto>(a);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var a = await repository.GetByIdAsync(id, ct);
        if (a is null) return false;

        a.IsActive = false;
        await repository.SaveChangesAsync(ct);
        return true;
    }
}
