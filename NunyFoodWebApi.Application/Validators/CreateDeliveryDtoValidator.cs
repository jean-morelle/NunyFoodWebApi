using FluentValidation;
using NunyFoodWebApi.Application.DTOs.Deliveries;

namespace NunyFoodWebApi.Application.Validators;

public class CreateDeliveryDtoValidator : AbstractValidator<CreateDeliveryDto>
{
    public CreateDeliveryDtoValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.DeliveryAgentId).NotEmpty();
        RuleFor(x => x.ReceiverName).NotEmpty().MaximumLength(200);
    }
}
