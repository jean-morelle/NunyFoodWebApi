using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NunyFoodWebApi.Application.DTOs.Admins;
using NunyFoodWebApi.Application.DTOs.Auth;
using NunyFoodWebApi.Application.Interfaces;

namespace NunyFoodWebApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    IAuthService authService,
    IAdminService adminService,
    IValidator<LoginDto> loginValidator,
    IValidator<RegisterAdminDto> registerAdminValidator) : ControllerBase
{
    [HttpPost("customer/login")]
    public async Task<IActionResult> CustomerLogin([FromBody] LoginDto dto, CancellationToken ct)
    {
        var result = await loginValidator.ValidateAsync(dto, ct);
        if (!result.IsValid)
        {
            foreach (var e in result.Errors) ModelState.AddModelError(e.PropertyName, e.ErrorMessage);
            return ValidationProblem();
        }

        var token = await authService.LoginCustomerAsync(dto, ct);
        return token is null ? Unauthorized() : Ok(token);
    }

    [HttpPost("admin/login")]
    public async Task<IActionResult> AdminLogin([FromBody] LoginDto dto, CancellationToken ct)
    {
        var result = await loginValidator.ValidateAsync(dto, ct);
        if (!result.IsValid)
        {
            foreach (var e in result.Errors) ModelState.AddModelError(e.PropertyName, e.ErrorMessage);
            return ValidationProblem();
        }

        var token = await authService.LoginAdminAsync(dto, ct);
        return token is null ? Unauthorized() : Ok(token);
    }

    [HttpPost("agent/login")]
    public async Task<IActionResult> AgentLogin([FromBody] LoginDto dto, CancellationToken ct)
    {
        var result = await loginValidator.ValidateAsync(dto, ct);
        if (!result.IsValid)
        {
            foreach (var e in result.Errors) ModelState.AddModelError(e.PropertyName, e.ErrorMessage);
            return ValidationProblem();
        }

        var token = await authService.LoginAgentAsync(dto, ct);
        return token is null ? Unauthorized() : Ok(token);
    }

    [HttpPost("admin/register")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdminRegister([FromBody] RegisterAdminDto dto, CancellationToken ct)
    {
        var result = await registerAdminValidator.ValidateAsync(dto, ct);
        if (!result.IsValid)
        {
            foreach (var e in result.Errors) ModelState.AddModelError(e.PropertyName, e.ErrorMessage);
            return ValidationProblem();
        }

        var admin = await adminService.RegisterAsync(dto, ct);
        return Created($"api/auth/admin/{admin.Id}", admin);
    }
}
