using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NunyFoodWebApi.Application.Common;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.Features.Admins.Commands;
using NunyFoodWebApi.Application.Features.Auth.Commands;
using NunyFoodWebApi.RateLimiting;

namespace NunyFoodWebApi.Controllers;

[ApiController]
[Route("api/auth")]
[EnableRateLimiting(RateLimitPolicies.Auth)]
public class AuthController(ISender sender) : ControllerBase
{
    [HttpPost("customer/login")]
    public Task<IActionResult> CustomerLogin([FromBody] LoginCommand command, CancellationToken ct) =>
        Login(command with { Role = Roles.Customer }, ct);

    [HttpPost("admin/login")]
    public Task<IActionResult> AdminLogin([FromBody] LoginCommand command, CancellationToken ct) =>
        Login(command with { Role = Roles.Admin }, ct);

    [HttpPost("agent/login")]
    public Task<IActionResult> AgentLogin([FromBody] LoginCommand command, CancellationToken ct) =>
        Login(command with { Role = Roles.DeliveryAgent }, ct);

    [HttpPost("customer/verify-otp")]
    public Task<IActionResult> CustomerVerifyOtp([FromBody] VerifyOtpCommand command, CancellationToken ct) =>
        VerifyOtp(command with { Role = Roles.Customer }, ct);

    [HttpPost("admin/verify-otp")]
    public Task<IActionResult> AdminVerifyOtp([FromBody] VerifyOtpCommand command, CancellationToken ct) =>
        VerifyOtp(command with { Role = Roles.Admin }, ct);

    [HttpPost("agent/verify-otp")]
    public Task<IActionResult> AgentVerifyOtp([FromBody] VerifyOtpCommand command, CancellationToken ct) =>
        VerifyOtp(command with { Role = Roles.DeliveryAgent }, ct);

    [HttpPost("customer/forgot-password")]
    [EnableRateLimiting(RateLimitPolicies.OtpEmail)]
    public async Task<IActionResult> CustomerForgotPassword([FromBody] ForgotPasswordCommand command, CancellationToken ct) =>
        Ok(await sender.Send(command, ct));

    [HttpPost("customer/reset-password")]
    public async Task<IActionResult> CustomerResetPassword([FromBody] ResetPasswordCommand command, CancellationToken ct)
    {
        await sender.Send(command, ct);
        return NoContent();
    }

    [HttpPost("admin/register")]
    [Authorize(Roles = Roles.Admin)]
    [DisableRateLimiting]
    public async Task<IActionResult> AdminRegister([FromBody] RegisterAdminCommand command, CancellationToken ct)
    {
        var admin = await sender.Send(command, ct);
        return Created($"api/auth/admin/{admin.Id}", admin);
    }

    private async Task<IActionResult> Login(LoginCommand command, CancellationToken ct)
    {
        var challenge = await sender.Send(command, ct);
        return challenge is null ? Unauthorized() : Ok(challenge);
    }

    private async Task<IActionResult> VerifyOtp(VerifyOtpCommand command, CancellationToken ct)
    {
        var token = await sender.Send(command, ct);
        return token is null ? Unauthorized() : Ok(token);
    }
}
