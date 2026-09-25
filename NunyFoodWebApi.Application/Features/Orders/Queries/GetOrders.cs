using AutoMapper;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.Orders;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Features.Orders.Queries;

/// <summary>Toutes les commandes, ou celles d'un client si <see cref="CustomerId"/> est fourni.</summary>
public record GetOrdersQuery(Guid? CustomerId) : IQuery<IEnumerable<OrderDto>>;

public class GetOrdersQueryHandler(IRepository<Order> repository, ICurrentUser currentUser, IMapper mapper)
    : IRequestHandler<GetOrdersQuery, IEnumerable<OrderDto>>
{
    public async Task<IEnumerable<OrderDto>> Handle(GetOrdersQuery request, CancellationToken ct)
    {
        // Un client ne voit que ses propres commandes, quel que soit le paramètre envoyé.
        var customerId = currentUser.IsCustomer ? currentUser.UserId : request.CustomerId;

        var list = customerId.HasValue
            ? await repository.FindAsync(o => o.CustomerId == customerId.Value, ct)
            : await repository.GetAllAsync(ct);
        return mapper.Map<IEnumerable<OrderDto>>(list);
    }
}
