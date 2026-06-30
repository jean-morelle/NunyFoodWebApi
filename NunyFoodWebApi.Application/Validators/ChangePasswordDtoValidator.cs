using FluentValidation;
using NunyFoodWebApi.Application.DTOs.Customers;

namespace NunyFoodWebApi.Application.Validators;

public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordDtoValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8).MaximumLength(100)
            .NotEqual(x => x.CurrentPassword).WithMessage("Le nouveau mot de passe doit être différent de l'actuel.");
    }
}
