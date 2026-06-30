using FluentValidation;
using NunyFoodWebApi.Application.DTOs.PackProducts;

namespace NunyFoodWebApi.Application.Validators;

public class AddProductToPackDtoValidator : AbstractValidator<AddProductToPackDto>
{
    public AddProductToPackDtoValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
