using AutoMapper;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.Products;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Features.Products.Queries;

public record GetProductsQuery : IQuery<IEnumerable<ProductDto>>;

public class GetProductsQueryHandler(IRepository<Product> repository, IMapper mapper)
    : IRequestHandler<GetProductsQuery, IEnumerable<ProductDto>>
{
    public async Task<IEnumerable<ProductDto>> Handle(GetProductsQuery request, CancellationToken ct)
    {
        var list = await repository.GetAllAsync(ct);
        return mapper.Map<IEnumerable<ProductDto>>(list);
    }
}
