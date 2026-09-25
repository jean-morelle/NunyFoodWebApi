using AutoMapper;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.Orders;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;
using NunyFoodWebApi.Domain.Enums;

namespace NunyFoodWebApi.Application.Features.Orders.Commands;

/// <summary>
/// Le client confirme que son bénéficiaire a bien reçu le pack : la commande passe de Livrée à Confirmée.
/// Retourne null si la commande n'est pas la sienne.
/// </summary>
public record ConfirmReceptionCommand(Guid Id) : ICommand<OrderDto?>;

public class ConfirmReceptionCommandHandler(
    IRepository<Order> orderRepo,
    IRepository<OrderStatusHistory> historyRepo,
    ICurrentUser currentUser,
    IMapper mapper) : IRequestHandler<ConfirmReceptionCommand, OrderDto?>
{
    public async Task<OrderDto?> Handle(ConfirmReceptionCommand request, CancellationToken ct)
    {
        var order = await orderRepo.GetByIdAsync(request.Id, ct);
        if (order is null || order.CustomerId != currentUser.UserId) return null;

        order.ChangeStatus(OrderStatus.Confirmed, historyRepo);
        await orderRepo.SaveChangesAsync(ct);
        return mapper.Map<OrderDto>(order);
    }
}
