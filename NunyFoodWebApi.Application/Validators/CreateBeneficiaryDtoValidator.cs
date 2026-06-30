using FluentValidation;
using NunyFoodWebApi.Application.DTOs.Beneficiaries;

namespace NunyFoodWebApi.Application.Validators;

public class CreateBeneficiaryDtoValidator : AbstractValidator<CreateBeneficiaryDto>
{
    public CreateBeneficiaryDtoValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(500);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
    }
}
