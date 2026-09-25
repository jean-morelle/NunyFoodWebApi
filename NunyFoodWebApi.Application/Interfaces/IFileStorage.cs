using NunyFoodWebApi.Application.Common;

namespace NunyFoodWebApi.Application.Interfaces;

/// <summary>Stockage privé des fichiers envoyés : rien n'est accessible sans passer par l'API.</summary>
public interface IFileStorage
{
    /// <summary>Enregistre le fichier dans <paramref name="folder"/> et retourne son chemin de stockage.</summary>
    Task<string> SaveAsync(FileUpload file, string folder, CancellationToken ct = default);

    /// <summary>Ouvre un fichier enregistré, ou null s'il n'existe pas (ou si le chemin est invalide).</summary>
    Task<StoredFile?> OpenReadAsync(string path, CancellationToken ct = default);
}
