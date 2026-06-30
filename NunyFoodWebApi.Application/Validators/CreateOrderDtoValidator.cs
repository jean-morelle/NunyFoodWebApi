using FluentValidation;
using NunyFoodWebApi.Application.DTOs.Orders;

namespace NunyFoodWebApi.Application.Validators;

public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderDtoValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.BeneficiaryId).NotEmpty();
        RuleFor(x => x.PackId).NotEmpty();
    }
}
