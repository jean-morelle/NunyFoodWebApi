using FluentValidation;
using NunyFoodWebApi.Application.Common;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.Auth;
using NunyFoodWebApi.Domain.Enums;

namespace NunyFoodWebApi.Application.Features.Auth.Commands;

/// <summary>
/// Envoie un code de réinitialisation au client s'il existe.
/// La réponse est identique que le compte existe ou non, pour ne pas révéler quels emails sont inscrits.
/// </summary>
public record ForgotPasswordCommand(string Email) : ICommand<OtpChallengeDto>;

public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}

public class ForgotPasswordCommandHandler(AccountLookup accounts, OtpManager otp)
    : IRequestHandler<ForgotPasswordCommand, OtpChallengeDto>
{
    public async Task<OtpChallengeDto> Handle(ForgotPasswordCommand request, CancellationToken ct)
    {
        var account = await accounts.FindByEmailAsync(Roles.Customer, request.Email, ct);
        if (account is not null)
            return await otp.IssueAsync(account.Email, Roles.Customer, OtpPurpose.PasswordReset, ct);

        return new OtpChallengeDto(request.Email, (int)OtpManager.OtpLifetime.TotalSeconds);
    }
}
