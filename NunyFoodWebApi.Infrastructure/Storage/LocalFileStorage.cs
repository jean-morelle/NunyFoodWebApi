using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NunyFoodWebApi.Application.Common;
using NunyFoodWebApi.Application.Interfaces;

namespace NunyFoodWebApi.Infrastructure.Storage;

public class StorageSettings
{
    /// <summary>Dossier des fichiers envoyés, relatif à la racine de l'API.</summary>
    public string UploadsPath { get; set; } = "uploads";

    /// <summary>Préfixe d'URL sous lequel l'API sert ces fichiers.</summary>
    public string PublicBasePath { get; set; } = "/uploads";

    public string GetRootPath(IHostEnvironment env) => Path.GetFullPath(Path.Combine(env.ContentRootPath, UploadsPath));
}

/// <summary>
/// Stockage sur le disque du serveur. Implémentation de V1 : à remplacer par un stockage cloud
/// (Azure Blob, S3...) en implémentant <see cref="IFileStorage"/>, sans toucher au reste du code.
/// </summary>
public class LocalFileStorage(IOptions<StorageSettings> options, IHostEnvironment env) : IFileStorage
{
    private readonly StorageSettings _settings = options.Value;

    public async Task<string> SaveAsync(FileUpload file, string folder, CancellationToken ct = default)
    {
        if (!ImageFiles.Extensions.TryGetValue(file.ContentType, out var extension))
            throw new InvalidOperationException($"Type de fichier non supporté : {file.ContentType}.");

        // Nom aléatoire : impossible à deviner, et aucune partie du nom ne vient du client.
        var fileName = $"{Guid.NewGuid():N}{extension}";
        var directory = Path.Combine(_settings.GetRootPath(env), folder);
        Directory.CreateDirectory(directory);

        await using (var output = File.Create(Path.Combine(directory, fileName)))
            await file.Content.CopyToAsync(output, ct);

        return $"{_settings.PublicBasePath.TrimEnd('/')}/{folder}/{fileName}";
    }
}
