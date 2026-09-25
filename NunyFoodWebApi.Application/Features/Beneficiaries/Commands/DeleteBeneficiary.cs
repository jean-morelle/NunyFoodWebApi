using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Features.Beneficiaries.Commands;

/// <summary>Retourne false si le bénéficiaire n'existe pas ou appartient à un autre client.</summary>
public record DeleteBeneficiaryCommand(Guid Id) : ICommand<bool>;

public class DeleteBeneficiaryCommandHandler(
    IRepository<Beneficiary> repository,
    IRepository<Order> orderRepo,
    ICurrentUser currentUser) : IRequestHandler<DeleteBeneficiaryCommand, bool>
{
    public async Task<bool> Handle(DeleteBeneficiaryCommand request, CancellationToken ct)
    {
        var b = await repository.GetByIdAsync(request.Id, ct);
        if (b is null || !currentUser.CanAccessCustomer(b.CustomerId)) return false;

        // Les commandes référencent le bénéficiaire (FK Restrict) : on garde l'historique intact.
        if (await orderRepo.FirstOrDefaultAsync(o => o.BeneficiaryId == b.Id, ct) is not null)
            throw new InvalidOperationException("Ce bénéficiaire a déjà des commandes et ne peut pas être supprimé.");

        repository.Remove(b);
        await repository.SaveChangesAsync(ct);
        return true;
    }
}
