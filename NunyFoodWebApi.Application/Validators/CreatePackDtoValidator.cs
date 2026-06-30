using FluentValidation;
using NunyFoodWebApi.Application.DTOs.Packs;

namespace NunyFoodWebApi.Application.Validators;

public class CreatePackDtoValidator : AbstractValidator<CreatePackDto>
{
    public CreatePackDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Price).GreaterThan(0);
        When(x => x.ImageUrl is not null, () => RuleFor(x => x.ImageUrl).MaximumLength(500));
    }
}
