using System.Text.Json.Serialization;
using AutoMapper;
using FluentValidation;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.Packs;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Features.Packs.Commands;

public record UpdatePackCommand(
    [property: JsonIgnore] Guid Id,
    string? Name,
    string? Description,
    decimal? Price,
    string? ImageUrl,
    bool? IsActive) : ICommand<PackDto?>;

public class UpdatePackCommandValidator : AbstractValidator<UpdatePackCommand>
{
    public UpdatePackCommandValidator()
    {
        When(x => x.Name is not null, () => RuleFor(x => x.Name).NotEmpty().MaximumLength(200));
        When(x => x.Description is not null, () => RuleFor(x => x.Description).NotEmpty().MaximumLength(1000));
        When(x => x.Price is not null, () => RuleFor(x => x.Price).GreaterThan(0));
        When(x => x.ImageUrl is not null, () => RuleFor(x => x.ImageUrl).MaximumLength(500));
    }
}

public class UpdatePackCommandHandler(IRepository<Pack> repository, IMapper mapper)
    : IRequestHandler<UpdatePackCommand, PackDto?>
{
    public async Task<PackDto?> Handle(UpdatePackCommand request, CancellationToken ct)
    {
        var p = await repository.GetByIdAsync(request.Id, ct);
        if (p is null) return null;

        if (request.Name is not null) p.Name = request.Name;
        if (request.Description is not null) p.Description = request.Description;
        if (request.Price is not null) p.Price = request.Price.Value;
        if (request.ImageUrl is not null) p.ImageUrl = request.ImageUrl;
        if (request.IsActive is not null) p.IsActive = request.IsActive.Value;

        await repository.SaveChangesAsync(ct);
        return mapper.Map<PackDto>(p);
    }
}
