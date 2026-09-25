using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NunyFoodWebApi.Application.Common;
using NunyFoodWebApi.Application.Interfaces;

namespace NunyFoodWebApi.Infrastructure.Storage;

public class StorageSettings
{
    /// <summary>Dossier des fichiers envoyés, relatif à la racine de l'API. Il n'est pas servi publiquement.</summary>
    public string UploadsPath { get; set; } = "uploads";

    public string GetRootPath(IHostEnvironment env) => Path.GetFullPath(Path.Combine(env.ContentRootPath, UploadsPath));
}

/// <summary>
/// Stockage privé sur le disque du serveur. Implémentation de V1 : à remplacer par un stockage cloud
/// (Azure Blob, S3...) en implémentant <see cref="IFileStorage"/>, sans toucher au reste du code.
/// </summary>
public class LocalFileStorage(IOptions<StorageSettings> options, IHostEnvironment env) : IFileStorage
{
    private readonly string _root = options.Value.GetRootPath(env);

    public async Task<string> SaveAsync(FileUpload file, string folder, CancellationToken ct = default)
    {
        if (!ImageFiles.Extensions.TryGetValue(file.ContentType, out var extension))
            throw new InvalidOperationException($"Type de fichier non supporté : {file.ContentType}.");

        // Nom aléatoire : aucune partie du nom ne vient du client.
        var path = $"{folder.Trim('/')}/{Guid.NewGuid():N}{extension}";
        var fullPath = ResolveInsideRoot(path)
            ?? throw new InvalidOperationException($"Dossier de stockage invalide : {folder}.");

        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        await using (var output = File.Create(fullPath))
            await file.Content.CopyToAsync(output, ct);

        return path;
    }

    public Task<StoredFile?> OpenReadAsync(string path, CancellationToken ct = default)
    {
        var fullPath = ResolveInsideRoot(path);
        var contentType = ImageFiles.ContentTypeOf(path);
        if (fullPath is null || contentType is null || !File.Exists(fullPath))
            return Task.FromResult<StoredFile?>(null);

        return Task.FromResult<StoredFile?>(new StoredFile(File.OpenRead(fullPath), contentType));
    }

    /// <summary>Chemin absolu du fichier, ou null s'il sortirait du dossier de stockage (ex. « ../ »).</summary>
    private string? ResolveInsideRoot(string path)
    {
        var fullPath = Path.GetFullPath(Path.Combine(_root, path));
        var rootWithSeparator = _root.EndsWith(Path.DirectorySeparatorChar) ? _root : _root + Path.DirectorySeparatorChar;
        return fullPath.StartsWith(rootWithSeparator, StringComparison.OrdinalIgnoreCase) ? fullPath : null;
    }
}
