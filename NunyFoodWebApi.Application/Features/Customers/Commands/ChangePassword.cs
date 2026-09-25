using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.Results;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Features.Customers.Commands;

/// <summary>Retourne false si le client est introuvable ou différent de l'utilisateur courant.</summary>
public record ChangePasswordCommand(
    [property: JsonIgnore] Guid CustomerId,
    string CurrentPassword,
    string NewPassword) : ICommand<bool>;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8).MaximumLength(100)
            .NotEqual(x => x.CurrentPassword).WithMessage("Le nouveau mot de passe doit être différent de l'actuel.");
    }
}

public class ChangePasswordCommandHandler(
    IRepository<Customer> repository,
    IPasswordHasher passwordHasher,
    ICurrentUser currentUser) : IRequestHandler<ChangePasswordCommand, bool>
{
    public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken ct)
    {
        // Seul le client lui-même peut changer son mot de passe (pas même un admin).
        if (currentUser.UserId != request.CustomerId) return false;

        var c = await repository.GetByIdAsync(request.CustomerId, ct);
        if (c is null) return false;

        // Erreur de validation (400) et non 401 : le frontend déconnecte l'utilisateur sur un 401.
        if (!passwordHasher.Verify(request.CurrentPassword, c.PasswordHash))
            throw new ValidationException([new ValidationFailure(nameof(request.CurrentPassword), "Mot de passe actuel incorrect.")]);

        c.PasswordHash = passwordHasher.Hash(request.NewPassword);
        await repository.SaveChangesAsync(ct);
        return true;
    }
}
