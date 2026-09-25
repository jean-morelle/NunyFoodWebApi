using AutoMapper;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.Packs;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Features.Packs.Queries;

public record GetPacksQuery : IQuery<IEnumerable<PackDto>>;

public class GetPacksQueryHandler(IRepository<Pack> repository, IMapper mapper)
    : IRequestHandler<GetPacksQuery, IEnumerable<PackDto>>
{
    public async Task<IEnumerable<PackDto>> Handle(GetPacksQuery request, CancellationToken ct)
    {
        var list = await repository.GetAllAsync(ct);
        return mapper.Map<IEnumerable<PackDto>>(list);
    }
}
