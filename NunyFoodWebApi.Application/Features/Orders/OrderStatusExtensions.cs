using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;
using NunyFoodWebApi.Domain.Enums;

namespace NunyFoodWebApi.Application.Features.Orders;

public static class OrderStatusExtensions
{
    /// <summary>Change le statut de la commande et historise le changement (sans sauvegarder).</summary>
    public static void ChangeStatus(this Order order, OrderStatus status, IRepository<OrderStatusHistory> historyRepo)
    {
        order.Status = status;
        historyRepo.Add(new OrderStatusHistory
        {
            OrderId = order.Id,
            Status = status,
            ChangedAt = DateTime.UtcNow
        });
    }
}
