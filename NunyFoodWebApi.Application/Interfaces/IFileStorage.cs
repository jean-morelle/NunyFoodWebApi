using NunyFoodWebApi.Application.Common;

namespace NunyFoodWebApi.Application.Interfaces;

public interface IFileStorage
{
    /// <summary>Enregistre le fichier dans <paramref name="folder"/> et retourne l'URL publique pour y accéder.</summary>
    Task<string> SaveAsync(FileUpload file, string folder, CancellationToken ct = default);
}
