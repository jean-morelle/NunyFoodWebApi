using AutoMapper;
using NunyFoodWebApi.Application.DTOs.Products;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Services;

public class ProductService(
    IRepository<Product> repository,
    IMapper mapper) : IProductService
{
    public async Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var p = await repository.GetByIdAsync(id, ct);
        return p is null ? null : mapper.Map<ProductDto>(p);
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await repository.GetAllAsync(ct);
        return mapper.Map<IEnumerable<ProductDto>>(list);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken ct = default)
    {
        var p = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            IsActive = true
        };
        repository.Add(p);
        await repository.SaveChangesAsync(ct);
        return mapper.Map<ProductDto>(p);
    }

    public async Task<ProductDto?> UpdateAsync(Guid id, UpdateProductDto dto, CancellationToken ct = default)
    {
        var p = await repository.GetByIdAsync(id, ct);
        if (p is null) return null;

        if (dto.Name is not null) p.Name = dto.Name;
        if (dto.Description is not null) p.Description = dto.Description;
        if (dto.IsActive is not null) p.IsActive = dto.IsActive.Value;

        await repository.SaveChangesAsync(ct);
        return mapper.Map<ProductDto>(p);
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
