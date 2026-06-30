using AutoMapper;
using NunyFoodWebApi.Application.DTOs.Packs;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Services;

public class PackService(
    IRepository<Pack> repository,
    IMapper mapper) : IPackService
{
    public async Task<PackDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var p = await repository.GetByIdAsync(id, ct);
        return p is null ? null : mapper.Map<PackDto>(p);
    }

    public async Task<IEnumerable<PackDto>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await repository.GetAllAsync(ct);
        return mapper.Map<IEnumerable<PackDto>>(list);
    }

    public async Task<PackDto> CreateAsync(CreatePackDto dto, CancellationToken ct = default)
    {
        var p = new Pack
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            ImageUrl = dto.ImageUrl,
            IsActive = true
        };
        repository.Add(p);
        await repository.SaveChangesAsync(ct);
        return mapper.Map<PackDto>(p);
    }

    public async Task<PackDto?> UpdateAsync(Guid id, UpdatePackDto dto, CancellationToken ct = default)
    {
        var p = await repository.GetByIdAsync(id, ct);
        if (p is null) return null;

        if (dto.Name is not null) p.Name = dto.Name;
        if (dto.Description is not null) p.Description = dto.Description;
        if (dto.Price is not null) p.Price = dto.Price.Value;
        if (dto.ImageUrl is not null) p.ImageUrl = dto.ImageUrl;
        if (dto.IsActive is not null) p.IsActive = dto.IsActive.Value;

        await repository.SaveChangesAsync(ct);
        return mapper.Map<PackDto>(p);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var p = await repository.GetByIdAsync(id, ct);
        if (p is null) return false;

        p.IsActive = false;
        await repository.SaveChangesAsync(ct);
        return true;
    }
}
