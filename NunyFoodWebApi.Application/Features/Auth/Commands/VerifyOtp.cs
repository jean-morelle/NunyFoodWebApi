using System.Text.Json.Serialization;
using FluentValidation;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.Auth;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Enums;

namespace NunyFoodWebApi.Application.Features.Auth.Commands;

/// <summary>
/// Seconde étape de connexion : échange le code OTP contre un JWT.
/// Retourne null si le code est invalide ou expiré.
/// </summary>
public record VerifyOtpCommand(
    string Email,
    string Code,
    [property: JsonIgnore] string Role = "") : ICommand<TokenDto?>;

public class VerifyOtpCommandValidator : AbstractValidator<VerifyOtpCommand>
{
    public VerifyOtpCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Code).NotEmpty().Length(6).Matches(@"^\d{6}$");
    }
}

public class VerifyOtpCommandHandler(AccountLookup accounts, OtpManager otp, IJwtTokenGenerator tokenGenerator)
    : IRequestHandler<VerifyOtpCommand, TokenDto?>
{
    private const int TokenLifetimeSeconds = 3600;

    public async Task<TokenDto?> Handle(VerifyOtpCommand request, CancellationToken ct)
    {
        if (!await otp.ConsumeAsync(request.Email, request.Code, request.Role, OtpPurpose.Login, ct))
            return null;

        var account = await accounts.FindByEmailAsync(request.Role, request.Email, ct);
        return account is null
            ? null
            : new TokenDto(tokenGenerator.GenerateToken(account.Id, account.Email, request.Role), "Bearer", TokenLifetimeSeconds);
    }
}
