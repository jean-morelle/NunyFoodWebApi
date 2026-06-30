using NunyFoodWebApi.Application.DTOs.Auth;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Services;

public class AuthService(
    IRepository<Customer> customerRepo,
    IRepository<Admin> adminRepo,
    IRepository<DeliveryAgent> agentRepo,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator tokenGenerator) : IAuthService
{
    public async Task<TokenDto?> LoginCustomerAsync(LoginDto dto, CancellationToken ct = default)
    {
        var customer = await customerRepo.FirstOrDefaultAsync(c => c.Email == dto.Email, ct);
        if (customer is null || !passwordHasher.Verify(dto.Password, customer.PasswordHash))
            return null;

        var token = tokenGenerator.GenerateToken(customer.Id, customer.Email, "Customer");
        return new TokenDto(token, "Bearer", 3600);
    }

    public async Task<TokenDto?> LoginAdminAsync(LoginDto dto, CancellationToken ct = default)
    {
        var admin = await adminRepo.FirstOrDefaultAsync(a => a.Email == dto.Email, ct);
        if (admin is null || !passwordHasher.Verify(dto.Password, admin.PasswordHash))
            return null;

        var token = tokenGenerator.GenerateToken(admin.Id, admin.Email, "Admin");
        return new TokenDto(token, "Bearer", 3600);
    }

    public async Task<TokenDto?> LoginAgentAsync(LoginDto dto, CancellationToken ct = default)
    {
        var agent = await agentRepo.FirstOrDefaultAsync(a => a.Email == dto.Email, ct);
        if (agent is null || !passwordHasher.Verify(dto.Password, agent.PasswordHash))
            return null;

        var token = tokenGenerator.GenerateToken(agent.Id, agent.Email, "DeliveryAgent");
        return new TokenDto(token, "Bearer", 3600);
    }
}
