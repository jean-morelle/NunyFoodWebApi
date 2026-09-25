using Microsoft.Extensions.DependencyInjection;
using NunyFoodWebApi.Application;
using NunyFoodWebApi.Application.Common;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Common;
using NunyFoodWebApi.Domain.Entities;
using NunyFoodWebApi.Infrastructure.Security;

namespace NunyFoodWebApi.Tests.Support;

/// <summary>
/// La vraie couche Application (médiateur, validation, handlers, AutoMapper) branchée sur des fakes :
/// les tests passent par ISender exactement comme les contrôleurs.
/// </summary>
public sealed class TestApp : IDisposable
{
    public const string Password = "Test@12345";

    private readonly ServiceProvider _provider;

    public FakeCurrentUser User { get; } = new();
    public FakeEmailService Emails { get; } = new();
    public FakePaymentProvider Payments { get; } = new();
    public FakeFileStorage Files { get; } = new();
    public IPasswordHasher Hasher { get; } = new PasswordHasher();

    public TestApp()
    {
        var services = new ServiceCollection();
        services.AddApplication();
        services.AddSingleton<InMemoryStore>();
        services.AddScoped(typeof(IRepository<>), typeof(InMemoryRepository<>));
        services.AddSingleton<ICurrentUser>(User);
        services.AddSingleton<IEmailService>(Emails);
        services.AddSingleton<IPaymentProvider>(Payments);
        services.AddSingleton<IFileStorage>(Files);
        services.AddSingleton(Hasher);
        services.AddSingleton<IJwtTokenGenerator, FakeJwtTokenGenerator>();
        services.AddScoped<IApplicationDbContext, UnsupportedDbContext>();
        _provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });
    }

    public IServiceProvider Services => _provider;

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request)
    {
        using var scope = _provider.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<ISender>().Send(request);
    }

    public List<T> Set<T>() where T : BaseEntity => _provider.GetRequiredService<InMemoryStore>().Set<T>();

    public T Add<T>(T entity) where T : BaseEntity
    {
        new InMemoryRepository<T>(_provider.GetRequiredService<InMemoryStore>()).Add(entity);
        return entity;
    }

    // --- Connexion simulée ---
    public void SignInAs(Guid id, string role) { User.Id = id; User.Role = role; }
    public void SignInAsAdmin() => SignInAs(Guid.CreateVersion7(), Roles.Admin);
    public void SignOut() { User.Id = null; User.Role = null; }

    // --- Données de départ ---
    public Customer AddCustomer(string email = "client@test.local") => Add(new Customer
    {
        FirstName = "Ama", LastName = "Test", Email = email, PhoneNumber = "+22890000000",
        PasswordHash = Hasher.Hash(Password)
    });

    public Beneficiary AddBeneficiary(Customer customer) => Add(new Beneficiary
    {
        CustomerId = customer.Id, FullName = "Mama", PhoneNumber = "+22891111111", Address = "Rue 1", City = "Lomé"
    });

    public Pack AddPack(decimal price = 15000) => Add(new Pack { Name = "Pack Famille", Description = "Riz, huile", Price = price });

    public DeliveryAgent AddAgent(string email = "livreur@test.local") => Add(new DeliveryAgent
    {
        FullName = "Kofi", Email = email, PhoneNumber = "+22892222222", Zone = "Lomé", PasswordHash = Hasher.Hash(Password)
    });

    public Order AddOrder(Customer customer, Beneficiary beneficiary, Pack pack, Domain.Enums.OrderStatus status = Domain.Enums.OrderStatus.Created) =>
        Add(new Order { CustomerId = customer.Id, BeneficiaryId = beneficiary.Id, PackId = pack.Id, Amount = pack.Price, Status = status });

    public void Dispose() => _provider.Dispose();
}
