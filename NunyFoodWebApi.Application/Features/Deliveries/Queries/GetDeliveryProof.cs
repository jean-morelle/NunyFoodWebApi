using NunyFoodWebApi.Application.Common;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Features.Deliveries.Queries;

public enum DeliveryProofKind { Photo, Signature }

/// <summary>
/// Fichier de preuve (photo ou signature) d'une livraison.
/// Retourne null si la livraison, le fichier ou le droit d'accès manque.
/// </summary>
public record GetDeliveryProofQuery(Guid DeliveryId, DeliveryProofKind Kind) : IQuery<StoredFile?>;

public class GetDeliveryProofQueryHandler(IRepository<Delivery> repository, DeliveryAccess access, IFileStorage storage)
    : IRequestHandler<GetDeliveryProofQuery, StoredFile?>
{
    public async Task<StoredFile?> Handle(GetDeliveryProofQuery request, CancellationToken ct)
    {
        var d = await repository.GetByIdAsync(request.DeliveryId, ct);
        if (d is null || !await access.CanViewAsync(d, ct)) return null;

        var path = request.Kind == DeliveryProofKind.Photo ? d.PhotoPath : d.SignaturePath;
        return path is null ? null : await storage.OpenReadAsync(path, ct);
    }
}
