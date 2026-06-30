using FluentValidation;
using NunyFoodWebApi.Application.DTOs.DeliveryAgents;

namespace NunyFoodWebApi.Application.Validators;

public class CreateDeliveryAgentDtoValidator : AbstractValidator<CreateDeliveryAgentDto>
{
    public CreateDeliveryAgentDtoValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Zone).NotEmpty().MaximumLength(100);
    }
}
