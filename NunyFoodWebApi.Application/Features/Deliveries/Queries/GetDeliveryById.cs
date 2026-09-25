using AutoMapper;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.Deliveries;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Features.Deliveries.Queries;

/// <summary>Retourne null si la livraison n'existe pas ou n'est pas visible par l'utilisateur courant.</summary>
public record GetDeliveryByIdQuery(Guid Id) : IQuery<DeliveryDto?>;

public class GetDeliveryByIdQueryHandler(IRepository<Delivery> repository, DeliveryAccess access, IMapper mapper)
    : IRequestHandler<GetDeliveryByIdQuery, DeliveryDto?>
{
    public async Task<DeliveryDto?> Handle(GetDeliveryByIdQuery request, CancellationToken ct)
    {
        var d = await repository.GetByIdAsync(request.Id, ct);
        return d is null || !await access.CanViewAsync(d, ct) ? null : mapper.Map<DeliveryDto>(d);
    }
}
