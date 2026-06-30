using FluentValidation;
using NunyFoodWebApi.Application.DTOs.Admins;

namespace NunyFoodWebApi.Application.Validators;

public class RegisterAdminDtoValidator : AbstractValidator<RegisterAdminDto>
{
    public RegisterAdminDtoValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
    }
}
