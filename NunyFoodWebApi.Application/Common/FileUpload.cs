namespace NunyFoodWebApi.Application.Common;

/// <summary>Fichier envoyé par le client, indépendant d'ASP.NET (IFormFile reste dans la couche API).</summary>
public sealed record FileUpload(Stream Content, string ContentType, long Length);

/// <summary>Fichier relu depuis le stockage, prêt à être renvoyé au client.</summary>
public sealed record StoredFile(Stream Content, string ContentType);

public static class ImageFiles
{
    public const long MaxBytes = 5 * 1024 * 1024;

    /// <summary>Types acceptés et extension utilisée à l'enregistrement (jamais celle fournie par le client).</summary>
    public static readonly IReadOnlyDictionary<string, string> Extensions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["image/jpeg"] = ".jpg",
        ["image/png"] = ".png",
        ["image/webp"] = ".webp"
    };

    public static string? ContentTypeOf(string path) =>
        Extensions.FirstOrDefault(e => path.EndsWith(e.Value, StringComparison.OrdinalIgnoreCase)).Key;
}
