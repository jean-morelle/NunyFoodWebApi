using FluentValidation;
using NunyFoodWebApi.Application.DTOs.Beneficiaries;

namespace NunyFoodWebApi.Application.Validators;

public class UpdateBeneficiaryDtoValidator : AbstractValidator<UpdateBeneficiaryDto>
{
    public UpdateBeneficiaryDtoValidator()
    {
        When(x => x.FullName is not null, () => RuleFor(x => x.FullName).NotEmpty().MaximumLength(200));
        When(x => x.PhoneNumber is not null, () => RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(20));
        When(x => x.Address is not null, () => RuleFor(x => x.Address).NotEmpty().MaximumLength(500));
        When(x => x.City is not null, () => RuleFor(x => x.City).NotEmpty().MaximumLength(100));
    }
}
