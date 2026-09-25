using System.Security.Cryptography;
using System.Text;
using NunyFoodWebApi.Application.DTOs.Auth;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;
using NunyFoodWebApi.Domain.Enums;

namespace NunyFoodWebApi.Application.Features.Auth;

/// <summary>
/// Émission et vérification des codes OTP envoyés par email (connexion, réinitialisation du mot de passe).
/// Un code n'est valable que pour l'objet (<see cref="OtpPurpose"/>) pour lequel il a été émis.
/// </summary>
public class OtpManager(IRepository<OtpCode> otpRepo, IEmailService emailService)
{
    public static readonly TimeSpan OtpLifetime = TimeSpan.FromMinutes(10);
    private const int MaxFailedAttempts = 5;

    public async Task<OtpChallengeDto> IssueAsync(string email, string role, OtpPurpose purpose, CancellationToken ct)
    {
        // Un seul code actif par compte et par objet : une nouvelle demande invalide la précédente.
        foreach (var previous in await otpRepo.FindAsync(o => o.Email == email && o.Role == role && o.Purpose == purpose, ct))
            otpRepo.Remove(previous);

        var code = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        otpRepo.Add(new OtpCode
        {
            Email = email,
            CodeHash = Hash(code),
            Role = role,
            Purpose = purpose,
            ExpiresAt = DateTime.UtcNow.Add(OtpLifetime)
        });
        await otpRepo.SaveChangesAsync(ct);

        await emailService.SendOtpAsync(email, code, purpose, ct);

        return new OtpChallengeDto(email, (int)OtpLifetime.TotalSeconds);
    }

    /// <summary>Vrai si le code est valide ; le code est alors consommé.</summary>
    public async Task<bool> ConsumeAsync(string email, string code, string role, OtpPurpose purpose, CancellationToken ct)
    {
        var otp = await otpRepo.FirstOrDefaultAsync(o => o.Email == email && o.Role == role && o.Purpose == purpose, ct);
        if (otp is null)
            return false;

        if (otp.ExpiresAt < DateTime.UtcNow)
        {
            otpRepo.Remove(otp);
            await otpRepo.SaveChangesAsync(ct);
            return false;
        }

        if (!CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(otp.CodeHash),
                Encoding.UTF8.GetBytes(Hash(code))))
        {
            // Au-delà de MaxFailedAttempts, le code est détruit pour bloquer la force brute.
            if (++otp.FailedAttempts >= MaxFailedAttempts)
                otpRepo.Remove(otp);
            await otpRepo.SaveChangesAsync(ct);
            return false;
        }

        otpRepo.Remove(otp);
        await otpRepo.SaveChangesAsync(ct);
        return true;
    }

    private static string Hash(string code) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(code)));
}
