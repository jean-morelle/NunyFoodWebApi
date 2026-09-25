using NunyFoodWebApi.Domain.Enums;

namespace NunyFoodWebApi.Application.Interfaces;

public interface IEmailService
{
    Task SendOtpAsync(string toEmail, string code, OtpPurpose purpose, CancellationToken ct = default);
}
