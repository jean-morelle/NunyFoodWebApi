using AutoMapper;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.Orders;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Features.Orders.Queries;

/// <summary>Retourne null si la commande n'existe pas ou appartient à un autre client.</summary>
public record GetOrderStatusHistoryQuery(Guid OrderId) : IQuery<IEnumerable<OrderStatusHistoryDto>?>;

public class GetOrderStatusHistoryQueryHandler(
    IRepository<Order> orderRepo,
    IRepository<OrderStatusHistory> historyRepo,
    ICurrentUser currentUser,
    IMapper mapper) : IRequestHandler<GetOrderStatusHistoryQuery, IEnumerable<OrderStatusHistoryDto>?>
{
    public async Task<IEnumerable<OrderStatusHistoryDto>?> Handle(GetOrderStatusHistoryQuery request, CancellationToken ct)
    {
        var order = await orderRepo.GetByIdAsync(request.OrderId, ct);
        if (order is null || !currentUser.CanAccessCustomer(order.CustomerId)) return null;

        var history = await historyRepo.FindAsync(h => h.OrderId == request.OrderId, ct);
        return mapper.Map<IEnumerable<OrderStatusHistoryDto>>(history.OrderBy(h => h.ChangedAt));
    }
}
