using System.Text.Json.Serialization;
using AutoMapper;
using FluentValidation;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.Beneficiaries;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Features.Beneficiaries.Commands;

/// <summary>Retourne null si le bénéficiaire n'existe pas ou appartient à un autre client.</summary>
public record UpdateBeneficiaryCommand(
    [property: JsonIgnore] Guid Id,
    string? FullName,
    string? PhoneNumber,
    string? Address,
    string? City) : ICommand<BeneficiaryDto?>;

public class UpdateBeneficiaryCommandValidator : AbstractValidator<UpdateBeneficiaryCommand>
{
    public UpdateBeneficiaryCommandValidator()
    {
        When(x => x.FullName is not null, () => RuleFor(x => x.FullName).NotEmpty().MaximumLength(200));
        When(x => x.PhoneNumber is not null, () => RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(20));
        When(x => x.Address is not null, () => RuleFor(x => x.Address).NotEmpty().MaximumLength(500));
        When(x => x.City is not null, () => RuleFor(x => x.City).NotEmpty().MaximumLength(100));
    }
}

public class UpdateBeneficiaryCommandHandler(IRepository<Beneficiary> repository, ICurrentUser currentUser, IMapper mapper)
    : IRequestHandler<UpdateBeneficiaryCommand, BeneficiaryDto?>
{
    public async Task<BeneficiaryDto?> Handle(UpdateBeneficiaryCommand request, CancellationToken ct)
    {
        var b = await repository.GetByIdAsync(request.Id, ct);
        if (b is null || !currentUser.CanAccessCustomer(b.CustomerId)) return null;

        if (request.FullName is not null) b.FullName = request.FullName;
        if (request.PhoneNumber is not null) b.PhoneNumber = request.PhoneNumber;
        if (request.Address is not null) b.Address = request.Address;
        if (request.City is not null) b.City = request.City;

        await repository.SaveChangesAsync(ct);
        return mapper.Map<BeneficiaryDto>(b);
    }
}
