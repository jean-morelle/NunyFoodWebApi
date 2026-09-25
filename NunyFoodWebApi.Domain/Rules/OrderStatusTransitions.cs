using NunyFoodWebApi.Domain.Enums;

namespace NunyFoodWebApi.Domain.Rules;

/// <summary>Cycle de vie d'une commande : quels changements de statut sont autorisés.</summary>
public static class OrderStatusTransitions
{
    private static readonly Dictionary<OrderStatus, OrderStatus[]> Allowed = new()
    {
        [OrderStatus.Created] = [OrderStatus.PendingPayment, OrderStatus.Paid, OrderStatus.Cancelled],
        [OrderStatus.PendingPayment] = [OrderStatus.Paid, OrderStatus.Cancelled],
        // Assigned est facultatif : l'affectation d'un livreur passe directement la commande en livraison.
        [OrderStatus.Paid] = [OrderStatus.Preparing, OrderStatus.Assigned, OrderStatus.InDelivery, OrderStatus.Cancelled],
        [OrderStatus.Preparing] = [OrderStatus.Assigned, OrderStatus.InDelivery, OrderStatus.Cancelled],
        [OrderStatus.Assigned] = [OrderStatus.InDelivery, OrderStatus.Cancelled],
        [OrderStatus.InDelivery] = [OrderStatus.Delivered],
        [OrderStatus.Delivered] = [OrderStatus.Confirmed],
        [OrderStatus.Confirmed] = [],
        [OrderStatus.Cancelled] = []
    };

    public static IReadOnlyList<OrderStatus> NextStatuses(OrderStatus from) => Allowed[from];

    public static bool CanTransition(OrderStatus from, OrderStatus to) => Allowed[from].Contains(to);
}
