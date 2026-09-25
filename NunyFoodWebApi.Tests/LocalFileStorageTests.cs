using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NunyFoodWebApi.Application.Common;
using NunyFoodWebApi.Infrastructure.Storage;

namespace NunyFoodWebApi.Tests;

public sealed class LocalFileStorageTests : IDisposable
{
    private readonly string _contentRoot = Directory.CreateTempSubdirectory("nunyfood-tests-").FullName;
    private readonly LocalFileStorage _storage;

    public LocalFileStorageTests() =>
        _storage = new LocalFileStorage(Options.Create(new StorageSettings()), new TestHostEnvironment(_contentRoot));

    private static FileUpload Png(byte[] content) => new(new MemoryStream(content), "image/png", content.Length);

    [Fact]
    public async Task Saved_file_can_be_read_back_with_its_content_type()
    {
        var path = await _storage.SaveAsync(Png([1, 2, 3]), "deliveries/abc");

        Assert.StartsWith("deliveries/abc/", path);
        Assert.EndsWith(".png", path);
        var file = await _storage.OpenReadAsync(path);
        Assert.NotNull(file);
        Assert.Equal("image/png", file.ContentType);
        await using var content = file.Content;
        using var copy = new MemoryStream();
        await content.CopyToAsync(copy);
        Assert.Equal([1, 2, 3], copy.ToArray());
    }

    [Fact]
    public async Task Files_are_not_stored_under_a_public_web_folder()
    {
        var path = await _storage.SaveAsync(Png([1]), "deliveries/abc");

        Assert.True(File.Exists(Path.Combine(_contentRoot, "uploads", path)));
        Assert.False(Directory.Exists(Path.Combine(_contentRoot, "wwwroot")));
    }

    [Theory]
    [InlineData("../appsettings.json")]
    [InlineData("deliveries/../../secret.png")]
    public async Task Paths_outside_the_storage_folder_are_refused(string path)
    {
        // Un fichier existe bien à côté du dossier de stockage : il ne doit pas être lisible.
        await File.WriteAllTextAsync(Path.Combine(_contentRoot, "secret.png"), "secret");

        Assert.Null(await _storage.OpenReadAsync(path));
        await Assert.ThrowsAsync<InvalidOperationException>(() => _storage.SaveAsync(Png([1]), path));
    }

    [Fact]
    public async Task Unknown_files_and_types_return_null()
    {
        Assert.Null(await _storage.OpenReadAsync("deliveries/absent.png"));
        Assert.Null(await _storage.OpenReadAsync("deliveries/fichier.exe"));
    }

    public void Dispose() => Directory.Delete(_contentRoot, recursive: true);

    private sealed class TestHostEnvironment(string contentRoot) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = "Test";
        public string ApplicationName { get; set; } = "NunyFoodWebApi.Tests";
        public string ContentRootPath { get; set; } = contentRoot;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
