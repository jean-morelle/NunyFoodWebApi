using AutoMapper;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.Packs;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Features.Packs.Queries;

public record GetPackByIdQuery(Guid Id) : IQuery<PackDto?>;

public class GetPackByIdQueryHandler(IRepository<Pack> repository, IMapper mapper)
    : IRequestHandler<GetPackByIdQuery, PackDto?>
{
    public async Task<PackDto?> Handle(GetPackByIdQuery request, CancellationToken ct)
    {
        var p = await repository.GetByIdAsync(request.Id, ct);
        return p is null ? null : mapper.Map<PackDto>(p);
    }
}
