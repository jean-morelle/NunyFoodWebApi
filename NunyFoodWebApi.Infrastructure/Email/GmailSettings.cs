namespace NunyFoodWebApi.Infrastructure.Email;

public class GmailSettings
{
    public string FromEmail { get; set; } = string.Empty;
    public string AppPassword { get; set; } = string.Empty;
    public string DisplayName { get; set; } = "NunyFood";

    /// <summary>
    /// Développement uniquement : écrit le code dans les logs au lieu de l'envoyer
    /// (utile pour les comptes seedés comme admin@nunyfood.com qui n'ont pas de vraie boîte mail).
    /// </summary>
    public bool LogCodesInsteadOfSending { get; set; }
}
