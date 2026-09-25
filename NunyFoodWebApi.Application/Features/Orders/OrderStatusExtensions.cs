using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;
using NunyFoodWebApi.Domain.Enums;
using NunyFoodWebApi.Domain.Rules;

namespace NunyFoodWebApi.Application.Features.Orders;

public static class OrderStatusExtensions
{
    /// <summary>
    /// Change le statut de la commande et historise le changement (sans sauvegarder).
    /// Lève <see cref="InvalidOperationException"/> (400) si le cycle de vie ne l'autorise pas.
    /// </summary>
    public static void ChangeStatus(this Order order, OrderStatus status, IRepository<OrderStatusHistory> historyRepo)
    {
        if (!OrderStatusTransitions.CanTransition(order.Status, status))
            throw new InvalidOperationException(
                $"Impossible de passer une commande « {order.Status.Label()} » à « {status.Label()} ».");

        order.Status = status;
        historyRepo.Add(new OrderStatusHistory
        {
            OrderId = order.Id,
            Status = status,
            ChangedAt = DateTime.UtcNow
        });
    }

    public static string Label(this OrderStatus status) => status switch
    {
        OrderStatus.Created => "Créée",
        OrderStatus.PendingPayment => "En attente de paiement",
        OrderStatus.Paid => "Payée",
        OrderStatus.Preparing => "En préparation",
        OrderStatus.Assigned => "Assignée",
        OrderStatus.InDelivery => "En livraison",
        OrderStatus.Delivered => "Livrée",
        OrderStatus.Confirmed => "Confirmée",
        OrderStatus.Cancelled => "Annulée",
        _ => status.ToString()
    };
}
