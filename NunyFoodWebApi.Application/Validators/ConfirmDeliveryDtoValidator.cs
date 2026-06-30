using FluentValidation;
using NunyFoodWebApi.Application.DTOs.Deliveries;

namespace NunyFoodWebApi.Application.Validators;

public class ConfirmDeliveryDtoValidator : AbstractValidator<ConfirmDeliveryDto>
{
    public ConfirmDeliveryDtoValidator()
    {
        RuleFor(x => x.ReceiverName).NotEmpty().MaximumLength(200);
        When(x => x.Latitude is not null, () => RuleFor(x => x.Latitude).InclusiveBetween(-90, 90));
        When(x => x.Longitude is not null, () => RuleFor(x => x.Longitude).InclusiveBetween(-180, 180));
    }
}
