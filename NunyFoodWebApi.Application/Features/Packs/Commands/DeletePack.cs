using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Features.Packs.Commands;

/// <summary>Suppression logique : le pack est désactivé.</summary>
public record DeletePackCommand(Guid Id) : ICommand<bool>;

public class DeletePackCommandHandler(IRepository<Pack> repository)
    : IRequestHandler<DeletePackCommand, bool>
{
    public async Task<bool> Handle(DeletePackCommand request, CancellationToken ct)
    {
        var p = await repository.GetByIdAsync(request.Id, ct);
        if (p is null) return false;

        p.IsActive = false;
        await repository.SaveChangesAsync(ct);
        return true;
    }
}
