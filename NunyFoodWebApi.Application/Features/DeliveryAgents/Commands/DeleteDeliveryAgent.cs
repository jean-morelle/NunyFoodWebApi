using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Features.DeliveryAgents.Commands;

/// <summary>Suppression logique : le livreur est désactivé.</summary>
public record DeleteDeliveryAgentCommand(Guid Id) : ICommand<bool>;

public class DeleteDeliveryAgentCommandHandler(IRepository<DeliveryAgent> repository)
    : IRequestHandler<DeleteDeliveryAgentCommand, bool>
{
    public async Task<bool> Handle(DeleteDeliveryAgentCommand request, CancellationToken ct)
    {
        var a = await repository.GetByIdAsync(request.Id, ct);
        if (a is null) return false;

        a.IsActive = false;
        await repository.SaveChangesAsync(ct);
        return true;
    }
}
