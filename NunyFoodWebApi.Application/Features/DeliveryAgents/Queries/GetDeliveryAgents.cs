using AutoMapper;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.DeliveryAgents;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Features.DeliveryAgents.Queries;

public record GetDeliveryAgentsQuery : IQuery<IEnumerable<DeliveryAgentDto>>;

public class GetDeliveryAgentsQueryHandler(IRepository<DeliveryAgent> repository, IMapper mapper)
    : IRequestHandler<GetDeliveryAgentsQuery, IEnumerable<DeliveryAgentDto>>
{
    public async Task<IEnumerable<DeliveryAgentDto>> Handle(GetDeliveryAgentsQuery request, CancellationToken ct)
    {
        var list = await repository.GetAllAsync(ct);
        return mapper.Map<IEnumerable<DeliveryAgentDto>>(list);
    }
}
