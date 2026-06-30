using NunyFoodWebApi.Application.DTOs.PackProducts;

namespace NunyFoodWebApi.Application.Interfaces;

public interface IPackProductService
{
    Task<IEnumerable<PackProductDto>> GetByPackIdAsync(Guid packId, CancellationToken ct = default);
    Task<PackProductDto> AddAsync(Guid packId, AddProductToPackDto dto, CancellationToken ct = default);
    Task<bool> RemoveAsync(Guid packId, Guid productId, CancellationToken ct = default);
}
