using System.Globalization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace NunyFoodWebApi.RateLimiting;

public static class RateLimitPolicies
{
    /// <summary>Connexion, vérification OTP, réinitialisation : bloque la devinette de mots de passe et de codes.</summary>
    public const string Auth = "auth";

    /// <summary>Demandes qui envoient un email (mot de passe oublié) : bloque l'envoi massif.</summary>
    public const string OtpEmail = "otp-email";
}

public class RateLimitingSettings
{
    public int AuthPermitLimit { get; set; } = 20;
    public int AuthWindowSeconds { get; set; } = 60;
    public int OtpEmailPermitLimit { get; set; } = 5;
    public int OtpEmailWindowSeconds { get; set; } = 15 * 60;
}

public static class RateLimitingSetup
{
    /// <summary>
    /// Limites par adresse IP. Derrière un reverse proxy en production, activer UseForwardedHeaders
    /// pour que RemoteIpAddress soit l'IP du client et non celle du proxy.
    /// </summary>
    public static IServiceCollection AddNunyFoodRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection("RateLimiting").Get<RateLimitingSettings>() ?? new RateLimitingSettings();

        return services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddPolicy(RateLimitPolicies.Auth, http =>
                FixedWindowByIp(http, RateLimitPolicies.Auth, settings.AuthPermitLimit, settings.AuthWindowSeconds));

            options.AddPolicy(RateLimitPolicies.OtpEmail, http =>
                FixedWindowByIp(http, RateLimitPolicies.OtpEmail, settings.OtpEmailPermitLimit, settings.OtpEmailWindowSeconds));

            options.OnRejected = async (context, ct) =>
            {
                var retryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var delay)
                    ? (int)Math.Ceiling(delay.TotalSeconds)
                    : (int?)null;
                if (retryAfter is not null)
                    context.HttpContext.Response.Headers.RetryAfter = retryAfter.Value.ToString(CultureInfo.InvariantCulture);

                await context.HttpContext.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = StatusCodes.Status429TooManyRequests,
                    Title = "Trop de tentatives. Réessayez dans quelques minutes."
                }, ct);
            };
        });
    }

    private static RateLimitPartition<string> FixedWindowByIp(HttpContext http, string policy, int permitLimit, int windowSeconds) =>
        RateLimitPartition.GetFixedWindowLimiter(
            $"{policy}:{http.Connection.RemoteIpAddress}",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = permitLimit,
                Window = TimeSpan.FromSeconds(windowSeconds),
                QueueLimit = 0
            });
}
