using FluentValidation;
using NunyFoodWebApi.Application.DTOs.Auth;

namespace NunyFoodWebApi.Application.Validators;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}
