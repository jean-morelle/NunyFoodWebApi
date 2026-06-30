using AutoMapper;
using NunyFoodWebApi.Application.DTOs.Admins;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Services;

public class AdminService(
    IRepository<Admin> repository,
    IPasswordHasher passwordHasher,
    IMapper mapper) : IAdminService
{
    public async Task<AdminDto> RegisterAsync(RegisterAdminDto dto, CancellationToken ct = default)
    {
        var admin = new Admin
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = passwordHasher.Hash(dto.Password)
        };
        repository.Add(admin);
        await repository.SaveChangesAsync(ct);
        return mapper.Map<AdminDto>(admin);
    }
}
