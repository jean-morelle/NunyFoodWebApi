using Microsoft.EntityFrameworkCore;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.PackProducts;
using NunyFoodWebApi.Application.Interfaces;

namespace NunyFoodWebApi.Application.Features.PackProducts.Queries;

public record GetPackProductsQuery(Guid PackId) : IQuery<IEnumerable<PackProductDto>>;

public class GetPackProductsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetPackProductsQuery, IEnumerable<PackProductDto>>
{
    public async Task<IEnumerable<PackProductDto>> Handle(GetPackProductsQuery request, CancellationToken ct)
    {
        return await context.PackProducts
            .Where(pp => pp.PackId == request.PackId)
            .Select(pp => new PackProductDto(pp.PackId, pp.ProductId, pp.Quantity))
            .ToListAsync(ct);
    }
}
