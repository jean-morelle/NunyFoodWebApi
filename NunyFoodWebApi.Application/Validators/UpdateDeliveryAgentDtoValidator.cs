using FluentValidation;
using NunyFoodWebApi.Application.DTOs.DeliveryAgents;

namespace NunyFoodWebApi.Application.Validators;

public class UpdateDeliveryAgentDtoValidator : AbstractValidator<UpdateDeliveryAgentDto>
{
    public UpdateDeliveryAgentDtoValidator()
    {
        When(x => x.FullName is not null, () => RuleFor(x => x.FullName).NotEmpty().MaximumLength(200));
        When(x => x.PhoneNumber is not null, () => RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(20));
        When(x => x.Zone is not null, () => RuleFor(x => x.Zone).NotEmpty().MaximumLength(100));
    }
}
