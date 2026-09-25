using AutoMapper;
using FluentValidation;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.Packs;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Features.Packs.Commands;

public record CreatePackCommand(
    string Name,
    string Description,
    decimal Price,
    string? ImageUrl) : ICommand<PackDto>;

public class CreatePackCommandValidator : AbstractValidator<CreatePackCommand>
{
    public CreatePackCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Price).GreaterThan(0);
        When(x => x.ImageUrl is not null, () => RuleFor(x => x.ImageUrl).MaximumLength(500));
    }
}

public class CreatePackCommandHandler(IRepository<Pack> repository, IMapper mapper)
    : IRequestHandler<CreatePackCommand, PackDto>
{
    public async Task<PackDto> Handle(CreatePackCommand request, CancellationToken ct)
    {
        var p = new Pack
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            ImageUrl = request.ImageUrl,
            IsActive = true
        };
        repository.Add(p);
        await repository.SaveChangesAsync(ct);
        return mapper.Map<PackDto>(p);
    }
}
