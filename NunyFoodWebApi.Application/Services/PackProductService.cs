using Microsoft.EntityFrameworkCore;
using NunyFoodWebApi.Application.DTOs.PackProducts;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Services;

public class PackProductService(IApplicationDbContext context) : IPackProductService
{
    public async Task<IEnumerable<PackProductDto>> GetByPackIdAsync(Guid packId, CancellationToken ct = default)
    {
        return await context.PackProducts
            .Where(pp => pp.PackId == packId)
            .Select(pp => new PackProductDto(pp.PackId, pp.ProductId, pp.Quantity))
            .ToListAsync(ct);
    }

    public async Task<PackProductDto> AddAsync(Guid packId, AddProductToPackDto dto, CancellationToken ct = default)
    {
        var existing = await context.PackProducts
            .FirstOrDefaultAsync(pp => pp.PackId == packId && pp.ProductId == dto.ProductId, ct);

        if (existing is not null)
        {
            existing.Quantity = dto.Quantity;
        }
        else
        {
            existing = new PackProduct
            {
                PackId = packId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            };
            context.PackProducts.Add(existing);
        }

        await context.SaveChangesAsync(ct);
        return new PackProductDto(existing.PackId, existing.ProductId, existing.Quantity);
    }

    public async Task<bool> RemoveAsync(Guid packId, Guid productId, CancellationToken ct = default)
    {
        var pp = await context.PackProducts
            .FirstOrDefaultAsync(pp => pp.PackId == packId && pp.ProductId == productId, ct);

        if (pp is null) return false;

        context.PackProducts.Remove(pp);
        await context.SaveChangesAsync(ct);
        return true;
    }
}
