using AutoMapper;
using FluentValidation;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.Beneficiaries;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Features.Beneficiaries.Commands;

/// <summary>Pour un client, <see cref="CustomerId"/> est ignoré et remplacé par son propre identifiant.</summary>
public record CreateBeneficiaryCommand(
    Guid CustomerId,
    string FullName,
    string PhoneNumber,
    string Address,
    string City) : ICommand<BeneficiaryDto>;

public class CreateBeneficiaryCommandValidator : AbstractValidator<CreateBeneficiaryCommand>
{
    public CreateBeneficiaryCommandValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(500);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
    }
}

public class CreateBeneficiaryCommandHandler(IRepository<Beneficiary> repository, ICurrentUser currentUser, IMapper mapper)
    : IRequestHandler<CreateBeneficiaryCommand, BeneficiaryDto>
{
    public async Task<BeneficiaryDto> Handle(CreateBeneficiaryCommand request, CancellationToken ct)
    {
        var customerId = currentUser.IsCustomer ? currentUser.UserId : request.CustomerId;
        if (customerId == Guid.Empty)
            throw new InvalidOperationException("CustomerId est obligatoire.");

        var b = new Beneficiary
        {
            CustomerId = customerId,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            Address = request.Address,
            City = request.City
        };
        repository.Add(b);
        await repository.SaveChangesAsync(ct);
        return mapper.Map<BeneficiaryDto>(b);
    }
}
