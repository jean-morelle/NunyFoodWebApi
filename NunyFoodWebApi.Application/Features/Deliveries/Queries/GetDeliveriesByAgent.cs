using AutoMapper;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.Deliveries;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Features.Deliveries.Queries;

public record GetDeliveriesByAgentQuery(Guid AgentId) : IQuery<IEnumerable<DeliveryDto>>;

public class GetDeliveriesByAgentQueryHandler(IRepository<Delivery> repository, ICurrentUser currentUser, IMapper mapper)
    : IRequestHandler<GetDeliveriesByAgentQuery, IEnumerable<DeliveryDto>>
{
    public async Task<IEnumerable<DeliveryDto>> Handle(GetDeliveriesByAgentQuery request, CancellationToken ct)
    {
        // Un livreur ne voit que ses propres livraisons, quel que soit le paramètre envoyé.
        var agentId = currentUser.IsDeliveryAgent ? currentUser.UserId : request.AgentId;

        var list = await repository.FindAsync(d => d.DeliveryAgentId == agentId, ct);
        return mapper.Map<IEnumerable<DeliveryDto>>(list);
    }
}
