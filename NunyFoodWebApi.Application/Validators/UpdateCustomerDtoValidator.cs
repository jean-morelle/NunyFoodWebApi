using FluentValidation;
using NunyFoodWebApi.Application.DTOs.Customers;

namespace NunyFoodWebApi.Application.Validators;

public class UpdateCustomerDtoValidator : AbstractValidator<UpdateCustomerDto>
{
    public UpdateCustomerDtoValidator()
    {
        When(x => x.FirstName is not null, () => RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100));
        When(x => x.LastName is not null, () => RuleFor(x => x.LastName).NotEmpty().MaximumLength(100));
        When(x => x.PhoneNumber is not null, () => RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(20));
    }
}
