namespace NunyFoodWebApi.Application.Interfaces;

/// <summary>Utilisateur authentifié qui exécute la requête (lu depuis le JWT).</summary>
public interface ICurrentUser
{
    /// <summary>Identifiant du compte (claim "sub"). Lève une exception si la requête n'est pas authentifiée.</summary>
    Guid UserId { get; }

    bool IsCustomer { get; }
    bool IsDeliveryAgent { get; }

    /// <summary>Vrai sauf si l'utilisateur est un client différent de <paramref name="customerId"/>.</summary>
    bool CanAccessCustomer(Guid customerId) => !IsCustomer || UserId == customerId;

    /// <summary>Vrai sauf si l'utilisateur est un livreur différent de <paramref name="deliveryAgentId"/>.</summary>
    bool CanAccessDeliveryAgent(Guid deliveryAgentId) => !IsDeliveryAgent || UserId == deliveryAgentId;
}
