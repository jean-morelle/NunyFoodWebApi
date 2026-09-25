using AutoMapper;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.Orders;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;
using NunyFoodWebApi.Domain.Enums;

namespace NunyFoodWebApi.Application.Features.Orders.Commands;

/// <summary>
/// Le client annule sa commande tant qu'elle n'est pas payée (après paiement, l'annulation
/// implique un remboursement et passe par l'admin). Retourne null si la commande n'est pas la sienne.
/// </summary>
public record CancelOrderCommand(Guid Id) : ICommand<OrderDto?>;

public class CancelOrderCommandHandler(
    IRepository<Order> orderRepo,
    IRepository<OrderStatusHistory> historyRepo,
    ICurrentUser currentUser,
    IMapper mapper) : IRequestHandler<CancelOrderCommand, OrderDto?>
{
    public async Task<OrderDto?> Handle(CancelOrderCommand request, CancellationToken ct)
    {
        var order = await orderRepo.GetByIdAsync(request.Id, ct);
        if (order is null || order.CustomerId != currentUser.UserId) return null;

        if (order.Status is not (OrderStatus.Created or OrderStatus.PendingPayment))
            throw new InvalidOperationException(
                "Une commande payée ne peut plus être annulée depuis votre espace. Contactez le support.");

        order.ChangeStatus(OrderStatus.Cancelled, historyRepo);
        await orderRepo.SaveChangesAsync(ct);
        return mapper.Map<OrderDto>(order);
    }
}
