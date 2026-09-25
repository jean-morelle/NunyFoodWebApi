using Microsoft.EntityFrameworkCore;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.Interfaces;

namespace NunyFoodWebApi.Application.Features.PackProducts.Commands;

public record RemoveProductFromPackCommand(Guid PackId, Guid ProductId) : ICommand<bool>;

public class RemoveProductFromPackCommandHandler(IApplicationDbContext context)
    : IRequestHandler<RemoveProductFromPackCommand, bool>
{
    public async Task<bool> Handle(RemoveProductFromPackCommand request, CancellationToken ct)
    {
        var pp = await context.PackProducts
            .FirstOrDefaultAsync(pp => pp.PackId == request.PackId && pp.ProductId == request.ProductId, ct);

        if (pp is null) return false;

        context.PackProducts.Remove(pp);
        await context.SaveChangesAsync(ct);
        return true;
    }
}
