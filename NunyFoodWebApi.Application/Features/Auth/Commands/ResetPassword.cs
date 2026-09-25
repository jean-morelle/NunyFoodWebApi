using FluentValidation;
using FluentValidation.Results;
using NunyFoodWebApi.Application.Common;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;
using NunyFoodWebApi.Domain.Enums;

namespace NunyFoodWebApi.Application.Features.Auth.Commands;

/// <summary>Remplace le mot de passe du client à l'aide du code reçu par email.</summary>
public record ResetPasswordCommand(string Email, string Code, string NewPassword) : ICommand<bool>;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Code).NotEmpty().Length(6).Matches(@"^\d{6}$");
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8).MaximumLength(100);
    }
}

public class ResetPasswordCommandHandler(
    OtpManager otp,
    IRepository<Customer> customerRepo,
    IPasswordHasher passwordHasher) : IRequestHandler<ResetPasswordCommand, bool>
{
    public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken ct)
    {
        // Erreur de validation (400) et non 401 : le frontend déconnecte l'utilisateur sur un 401.
        if (!await otp.ConsumeAsync(request.Email, request.Code, Roles.Customer, OtpPurpose.PasswordReset, ct))
            throw new ValidationException([new ValidationFailure(nameof(request.Code), "Code invalide ou expiré.")]);

        var customer = await customerRepo.FirstOrDefaultAsync(c => c.Email == request.Email, ct)
            ?? throw new ValidationException([new ValidationFailure(nameof(request.Code), "Code invalide ou expiré.")]);

        customer.PasswordHash = passwordHasher.Hash(request.NewPassword);
        await customerRepo.SaveChangesAsync(ct);
        return true;
    }
}
