using System.Text.Json.Serialization;
using AutoMapper;
using FluentValidation;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.Products;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Features.Products.Commands;

public record UpdateProductCommand(
    [property: JsonIgnore] Guid Id,
    string? Name,
    string? Description,
    bool? IsActive) : ICommand<ProductDto?>;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        When(x => x.Name is not null, () => RuleFor(x => x.Name).NotEmpty().MaximumLength(200));
        When(x => x.Description is not null, () => RuleFor(x => x.Description).NotEmpty().MaximumLength(1000));
    }
}

public class UpdateProductCommandHandler(IRepository<Product> repository, IMapper mapper)
    : IRequestHandler<UpdateProductCommand, ProductDto?>
{
    public async Task<ProductDto?> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        var p = await repository.GetByIdAsync(request.Id, ct);
        if (p is null) return null;

        if (request.Name is not null) p.Name = request.Name;
        if (request.Description is not null) p.Description = request.Description;
        if (request.IsActive is not null) p.IsActive = request.IsActive.Value;

        await repository.SaveChangesAsync(ct);
        return mapper.Map<ProductDto>(p);
    }
}
