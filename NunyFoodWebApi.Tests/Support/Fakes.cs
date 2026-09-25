using NunyFoodWebApi.Application.Common;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Enums;

namespace NunyFoodWebApi.Tests.Support;

/// <summary>Utilisateur courant modifiable depuis le test (équivalent du JWT).</summary>
public sealed class FakeCurrentUser : ICurrentUser
{
    public Guid? Id { get; set; }
    public string? Role { get; set; }

    public Guid UserId => Id ?? throw new UnauthorizedAccessException("Aucun utilisateur connecté.");
    public bool IsCustomer => Role == Roles.Customer;
    public bool IsDeliveryAgent => Role == Roles.DeliveryAgent;
}

public sealed record SentOtp(string Email, string Code, OtpPurpose Purpose);

public sealed class FakeEmailService : IEmailService
{
    public List<SentOtp> Sent { get; } = [];

    public Task SendOtpAsync(string toEmail, string code, OtpPurpose purpose, CancellationToken ct = default)
    {
        Sent.Add(new SentOtp(toEmail, code, purpose));
        return Task.CompletedTask;
    }

    public string LastCodeFor(string email, OtpPurpose purpose) =>
        Sent.Last(s => s.Email == email && s.Purpose == purpose).Code;
}

/// <summary>Prestataire de paiement déterministe (le vrai FakePaymentProvider est aléatoire).</summary>
public sealed class FakePaymentProvider : IPaymentProvider
{
    public PaymentProviderStatus NextStatus { get; set; } = PaymentProviderStatus.Succeeded;

    public Task<PaymentProviderResult> ProcessAsync(decimal amount, PaymentMethod method, CancellationToken ct = default) =>
        Task.FromResult(new PaymentProviderResult(NextStatus, $"TX-{Guid.NewGuid():N}", null));
}

public sealed class FakeFileStorage : IFileStorage
{
    public List<string> Saved { get; } = [];

    public Task<string> SaveAsync(FileUpload file, string folder, CancellationToken ct = default)
    {
        var path = $"{folder}/{Saved.Count}.png";
        Saved.Add(path);
        return Task.FromResult(path);
    }

    public Task<StoredFile?> OpenReadAsync(string path, CancellationToken ct = default) =>
        Task.FromResult(Saved.Contains(path)
            ? new StoredFile(new MemoryStream("image"u8.ToArray()), "image/png")
            : null);
}

/// <summary>
/// Les handlers PackProducts utilisent EF directement (IApplicationDbContext) : non couverts par ces tests
/// en mémoire. Ce substitut permet seulement de construire les handlers.
/// </summary>
public sealed class UnsupportedDbContext : IApplicationDbContext
{
    private static NotSupportedException Unsupported() => new("IApplicationDbContext n'est pas disponible dans les tests en mémoire.");

    public Microsoft.EntityFrameworkCore.DbSet<Domain.Entities.Admin> Admins => throw Unsupported();
    public Microsoft.EntityFrameworkCore.DbSet<Domain.Entities.Customer> Customers => throw Unsupported();
    public Microsoft.EntityFrameworkCore.DbSet<Domain.Entities.Beneficiary> Beneficiaries => throw Unsupported();
    public Microsoft.EntityFrameworkCore.DbSet<Domain.Entities.Product> Products => throw Unsupported();
    public Microsoft.EntityFrameworkCore.DbSet<Domain.Entities.Pack> Packs => throw Unsupported();
    public Microsoft.EntityFrameworkCore.DbSet<Domain.Entities.PackProduct> PackProducts => throw Unsupported();
    public Microsoft.EntityFrameworkCore.DbSet<Domain.Entities.Order> Orders => throw Unsupported();
    public Microsoft.EntityFrameworkCore.DbSet<Domain.Entities.Payment> Payments => throw Unsupported();
    public Microsoft.EntityFrameworkCore.DbSet<Domain.Entities.OrderStatusHistory> OrderStatusHistories => throw Unsupported();
    public Microsoft.EntityFrameworkCore.DbSet<Domain.Entities.DeliveryAgent> DeliveryAgents => throw Unsupported();
    public Microsoft.EntityFrameworkCore.DbSet<Domain.Entities.Delivery> Deliveries => throw Unsupported();
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => throw Unsupported();
}

public sealed class FakeJwtTokenGenerator : IJwtTokenGenerator
{
    public string GenerateToken(Guid userId, string email, string role) => $"token:{userId}:{role}";
}
