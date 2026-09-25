using System.Text.Json.Serialization;
using FluentValidation;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.Auth;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Enums;

namespace NunyFoodWebApi.Application.Features.Auth.Commands;

/// <summary>
/// Première étape de connexion : vérifie le mot de passe puis envoie un code OTP.
/// Retourne null si les identifiants sont invalides.
/// </summary>
public record LoginCommand(
    string Email,
    string Password,
    [property: JsonIgnore] string Role = "") : ICommand<OtpChallengeDto?>;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class LoginCommandHandler(AccountLookup accounts, IPasswordHasher passwordHasher, OtpManager otp)
    : IRequestHandler<LoginCommand, OtpChallengeDto?>
{
    public async Task<OtpChallengeDto?> Handle(LoginCommand request, CancellationToken ct)
    {
        var account = await accounts.FindByEmailAsync(request.Role, request.Email, ct);
        if (account is null || !passwordHasher.Verify(request.Password, account.PasswordHash))
            return null;

        return await otp.IssueAsync(account.Email, request.Role, OtpPurpose.Login, ct);
    }
}
