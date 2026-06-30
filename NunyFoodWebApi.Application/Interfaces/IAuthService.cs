using NunyFoodWebApi.Application.DTOs.Auth;

namespace NunyFoodWebApi.Application.Interfaces;

public interface IAuthService
{
    Task<TokenDto?> LoginCustomerAsync(LoginDto dto, CancellationToken ct = default);
    Task<TokenDto?> LoginAdminAsync(LoginDto dto, CancellationToken ct = default);
    Task<TokenDto?> LoginAgentAsync(LoginDto dto, CancellationToken ct = default);
}
