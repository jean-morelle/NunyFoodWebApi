using AutoMapper;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.Deliveries;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Features.Deliveries.Queries;

/// <summary>Retourne null si la livraison n'existe pas ou est affectée à un autre livreur.</summary>
public record GetDeliveryByIdQuery(Guid Id) : IQuery<DeliveryDto?>;

public class GetDeliveryByIdQueryHandler(IRepository<Delivery> repository, ICurrentUser currentUser, IMapper mapper)
    : IRequestHandler<GetDeliveryByIdQuery, DeliveryDto?>
{
    public async Task<DeliveryDto?> Handle(GetDeliveryByIdQuery request, CancellationToken ct)
    {
        var d = await repository.GetByIdAsync(request.Id, ct);
        return d is null || !currentUser.CanAccessDeliveryAgent(d.DeliveryAgentId) ? null : mapper.Map<DeliveryDto>(d);
    }
}
