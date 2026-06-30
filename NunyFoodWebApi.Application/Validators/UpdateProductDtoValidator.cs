using FluentValidation;
using NunyFoodWebApi.Application.DTOs.Products;

namespace NunyFoodWebApi.Application.Validators;

public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductDtoValidator()
    {
        When(x => x.Name is not null, () => RuleFor(x => x.Name).NotEmpty().MaximumLength(200));
        When(x => x.Description is not null, () => RuleFor(x => x.Description).NotEmpty().MaximumLength(1000));
    }
}
