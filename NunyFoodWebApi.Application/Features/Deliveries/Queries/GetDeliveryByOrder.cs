using AutoMapper;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.Deliveries;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Features.Deliveries.Queries;

/// <summary>Retourne null si la commande n'a pas de livraison ou si elle n'est pas visible par l'utilisateur courant.</summary>
public record GetDeliveryByOrderQuery(Guid OrderId) : IQuery<DeliveryDto?>;

public class GetDeliveryByOrderQueryHandler(IRepository<Delivery> repository, DeliveryAccess access, IMapper mapper)
    : IRequestHandler<GetDeliveryByOrderQuery, DeliveryDto?>
{
    public async Task<DeliveryDto?> Handle(GetDeliveryByOrderQuery request, CancellationToken ct)
    {
        var d = await repository.FirstOrDefaultAsync(d => d.OrderId == request.OrderId, ct);
        return d is null || !await access.CanViewAsync(d, ct) ? null : mapper.Map<DeliveryDto>(d);
    }
}
