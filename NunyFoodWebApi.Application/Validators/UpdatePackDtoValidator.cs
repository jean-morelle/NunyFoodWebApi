using FluentValidation;
using NunyFoodWebApi.Application.DTOs.Packs;

namespace NunyFoodWebApi.Application.Validators;

public class UpdatePackDtoValidator : AbstractValidator<UpdatePackDto>
{
    public UpdatePackDtoValidator()
    {
        When(x => x.Name is not null, () => RuleFor(x => x.Name).NotEmpty().MaximumLength(200));
        When(x => x.Description is not null, () => RuleFor(x => x.Description).NotEmpty().MaximumLength(1000));
        When(x => x.Price is not null, () => RuleFor(x => x.Price).GreaterThan(0));
        When(x => x.ImageUrl is not null, () => RuleFor(x => x.ImageUrl).MaximumLength(500));
    }
}
