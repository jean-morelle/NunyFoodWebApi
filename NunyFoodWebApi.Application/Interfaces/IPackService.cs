using NunyFoodWebApi.Application.DTOs.Packs;

namespace NunyFoodWebApi.Application.Interfaces;

public interface IPackService
{
    Task<PackDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<PackDto>> GetAllAsync(CancellationToken ct = default);
    Task<PackDto> CreateAsync(CreatePackDto dto, CancellationToken ct = default);
    Task<PackDto?> UpdateAsync(Guid id, UpdatePackDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
