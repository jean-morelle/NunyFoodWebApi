using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Features.Deliveries;

/// <summary>
/// Qui peut consulter une livraison (et sa preuve) : le livreur affecté, le client qui a passé
/// la commande, et l'admin. Les rôles eux-mêmes sont filtrés en amont par les contrôleurs.
/// </summary>
public class DeliveryAccess(IRepository<Order> orderRepo, ICurrentUser currentUser)
{
    public async Task<bool> CanViewAsync(Delivery delivery, CancellationToken ct)
    {
        if (currentUser.IsDeliveryAgent)
            return delivery.DeliveryAgentId == currentUser.UserId;

        if (currentUser.IsCustomer)
        {
            var order = await orderRepo.GetByIdAsync(delivery.OrderId, ct);
            return order?.CustomerId == currentUser.UserId;
        }

        return true; // Admin
    }
}
