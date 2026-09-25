using AutoMapper;
using FluentValidation;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.Admins;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Features.Admins.Commands;

public record RegisterAdminCommand(string FullName, string Email, string Password) : ICommand<AdminDto>;

public class RegisterAdminCommandValidator : AbstractValidator<RegisterAdminCommand>
{
    public RegisterAdminCommandValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
    }
}

public class RegisterAdminCommandHandler(
    IRepository<Admin> repository,
    IPasswordHasher passwordHasher,
    IMapper mapper) : IRequestHandler<RegisterAdminCommand, AdminDto>
{
    public async Task<AdminDto> Handle(RegisterAdminCommand request, CancellationToken ct)
    {
        var admin = new Admin
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = passwordHasher.Hash(request.Password)
        };
        repository.Add(admin);
        await repository.SaveChangesAsync(ct);
        return mapper.Map<AdminDto>(admin);
    }
}
