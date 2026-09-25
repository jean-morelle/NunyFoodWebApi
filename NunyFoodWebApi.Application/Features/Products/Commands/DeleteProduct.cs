using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Features.Products.Commands;

/// <summary>Suppression logique : le produit est désactivé.</summary>
public record DeleteProductCommand(Guid Id) : ICommand<bool>;

public class DeleteProductCommandHandler(IRepository<Product> repository)
    : IRequestHandler<DeleteProductCommand, bool>
{
    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken ct)
    {
        var p = await repository.GetByIdAsync(request.Id, ct);
        if (p is null) return false;

        p.IsActive = false;
        await repository.SaveChangesAsync(ct);
        return true;
    }
}
