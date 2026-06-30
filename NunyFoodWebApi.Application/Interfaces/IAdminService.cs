using NunyFoodWebApi.Application.DTOs.Admins;

namespace NunyFoodWebApi.Application.Interfaces;

public interface IAdminService
{
    Task<AdminDto> RegisterAsync(RegisterAdminDto dto, CancellationToken ct = default);
}
