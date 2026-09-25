using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Enums;

namespace NunyFoodWebApi.Infrastructure.Email;

public class GmailEmailService(IOptions<GmailSettings> options, ILogger<GmailEmailService> logger) : IEmailService
{
    private readonly GmailSettings _settings = options.Value;

    public async Task SendOtpAsync(string toEmail, string code, OtpPurpose purpose, CancellationToken ct = default)
    {
        var isReset = purpose == OtpPurpose.PasswordReset;

        if (_settings.LogCodesInsteadOfSending)
        {
            if (isReset)
                logger.LogWarning("[DEV] Code de réinitialisation pour {Email} : {Code}", toEmail, code);
            else
                logger.LogWarning("[DEV] Code OTP pour {Email} : {Code}", toEmail, code);
            return;
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.DisplayName, _settings.FromEmail));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = isReset
            ? "Réinitialisation de votre mot de passe NunyFood"
            : "Votre code de vérification NunyFood";
        message.Body = new TextPart("plain")
        {
            Text = isReset
                ? $"Bonjour,\n\nVotre code de réinitialisation de mot de passe est : {code}\n\nCe code expire dans 10 minutes. Si vous n'êtes pas à l'origine de cette demande, ignorez cet email.\n\n— NunyFood"
                : $"Bonjour,\n\nVotre code de vérification est : {code}\n\nCe code expire dans 10 minutes.\n\n— NunyFood"
        };

        using var client = new SmtpClient();
        await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls, ct);
        await client.AuthenticateAsync(_settings.FromEmail, _settings.AppPassword, ct);
        await client.SendAsync(message, ct);
        await client.DisconnectAsync(true, ct);
    }
}
