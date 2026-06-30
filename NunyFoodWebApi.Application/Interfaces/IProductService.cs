using NunyFoodWebApi.Application.DTOs.Products;

namespace NunyFoodWebApi.Application.Interfaces;

public interface IProductService
{
    Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken ct = default);
    Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken ct = default);
    Task<ProductDto?> UpdateAsync(Guid id, UpdateProductDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
