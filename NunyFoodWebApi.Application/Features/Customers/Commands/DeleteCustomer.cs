using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Features.Customers.Commands;

public record DeleteCustomerCommand(Guid Id) : ICommand<bool>;

public class DeleteCustomerCommandHandler(IRepository<Customer> repository, IRepository<Order> orderRepo)
    : IRequestHandler<DeleteCustomerCommand, bool>
{
    public async Task<bool> Handle(DeleteCustomerCommand request, CancellationToken ct)
    {
        var c = await repository.GetByIdAsync(request.Id, ct);
        if (c is null) return false;

        // Les commandes référencent le client (FK Restrict) : on garde l'historique intact.
        if (await orderRepo.FirstOrDefaultAsync(o => o.CustomerId == c.Id, ct) is not null)
            throw new InvalidOperationException("Ce client a déjà des commandes et ne peut pas être supprimé.");

        repository.Remove(c);
        await repository.SaveChangesAsync(ct);
        return true;
    }
}
