using AutoMapper;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.Products;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Features.Products.Queries;

public record GetProductByIdQuery(Guid Id) : IQuery<ProductDto?>;

public class GetProductByIdQueryHandler(IRepository<Product> repository, IMapper mapper)
    : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken ct)
    {
        var p = await repository.GetByIdAsync(request.Id, ct);
        return p is null ? null : mapper.Map<ProductDto>(p);
    }
}
