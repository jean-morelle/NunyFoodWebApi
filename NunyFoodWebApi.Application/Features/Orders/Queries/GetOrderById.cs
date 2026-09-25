using AutoMapper;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.Orders;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Features.Orders.Queries;

/// <summary>Retourne null si la commande n'existe pas ou appartient à un autre client.</summary>
public record GetOrderByIdQuery(Guid Id) : IQuery<OrderDto?>;

public class GetOrderByIdQueryHandler(IRepository<Order> repository, ICurrentUser currentUser, IMapper mapper)
    : IRequestHandler<GetOrderByIdQuery, OrderDto?>
{
    public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken ct)
    {
        var o = await repository.GetByIdAsync(request.Id, ct);
        return o is null || !currentUser.CanAccessCustomer(o.CustomerId) ? null : mapper.Map<OrderDto>(o);
    }
}
