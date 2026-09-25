using AutoMapper;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.DeliveryAgents;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Features.DeliveryAgents.Queries;

public record GetDeliveryAgentByIdQuery(Guid Id) : IQuery<DeliveryAgentDto?>;

public class GetDeliveryAgentByIdQueryHandler(IRepository<DeliveryAgent> repository, IMapper mapper)
    : IRequestHandler<GetDeliveryAgentByIdQuery, DeliveryAgentDto?>
{
    public async Task<DeliveryAgentDto?> Handle(GetDeliveryAgentByIdQuery request, CancellationToken ct)
    {
        var a = await repository.GetByIdAsync(request.Id, ct);
        return a is null ? null : mapper.Map<DeliveryAgentDto>(a);
    }
}
