using AutoMapper;
using FluentValidation;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.Products;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Features.Products.Commands;

public record CreateProductCommand(string Name, string Description) : ICommand<ProductDto>;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
    }
}

public class CreateProductCommandHandler(IRepository<Product> repository, IMapper mapper)
    : IRequestHandler<CreateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var p = new Product
        {
            Name = request.Name,
            Description = request.Description,
            IsActive = true
        };
        repository.Add(p);
        await repository.SaveChangesAsync(ct);
        return mapper.Map<ProductDto>(p);
    }
}
