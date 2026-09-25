using NunyFoodWebApi.Application.Common;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Features.Auth;

public record Account(Guid Id, string Email, string PasswordHash);

/// <summary>Retrouve un compte (client, admin ou livreur) par email selon le rôle demandé.</summary>
public class AccountLookup(
    IRepository<Customer> customerRepo,
    IRepository<Admin> adminRepo,
    IRepository<DeliveryAgent> agentRepo)
{
    public async Task<Account?> FindByEmailAsync(string role, string email, CancellationToken ct) => role switch
    {
        Roles.Customer => await customerRepo.FirstOrDefaultAsync(c => c.Email == email, ct) is { } c
            ? new Account(c.Id, c.Email, c.PasswordHash) : null,
        Roles.Admin => await adminRepo.FirstOrDefaultAsync(a => a.Email == email, ct) is { } a
            ? new Account(a.Id, a.Email, a.PasswordHash) : null,
        Roles.DeliveryAgent => await agentRepo.FirstOrDefaultAsync(a => a.Email == email, ct) is { } d
            ? new Account(d.Id, d.Email, d.PasswordHash) : null,
        _ => throw new ArgumentOutOfRangeException(nameof(role), role, "Rôle inconnu.")
    };
}
