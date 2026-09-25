using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.PackProducts;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Features.PackProducts.Commands;

/// <summary>Ajoute le produit au pack, ou met à jour sa quantité s'il y est déjà.</summary>
public record AddProductToPackCommand(
    [property: JsonIgnore] Guid PackId,
    Guid ProductId,
    decimal Quantity) : ICommand<PackProductDto>;

public class AddProductToPackCommandValidator : AbstractValidator<AddProductToPackCommand>
{
    public AddProductToPackCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}

public class AddProductToPackCommandHandler(IApplicationDbContext context)
    : IRequestHandler<AddProductToPackCommand, PackProductDto>
{
    public async Task<PackProductDto> Handle(AddProductToPackCommand request, CancellationToken ct)
    {
        var existing = await context.PackProducts
            .FirstOrDefaultAsync(pp => pp.PackId == request.PackId && pp.ProductId == request.ProductId, ct);

        if (existing is not null)
        {
            existing.Quantity = request.Quantity;
        }
        else
        {
            existing = new PackProduct
            {
                PackId = request.PackId,
                ProductId = request.ProductId,
                Quantity = request.Quantity
            };
            context.PackProducts.Add(existing);
        }

        await context.SaveChangesAsync(ct);
        return new PackProductDto(existing.PackId, existing.ProductId, existing.Quantity);
    }
}
