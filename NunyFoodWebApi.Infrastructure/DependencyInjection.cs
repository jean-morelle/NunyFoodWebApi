using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Infrastructure.Payments;
using NunyFoodWebApi.Infrastructure.Persistence;
using NunyFoodWebApi.Infrastructure.Security;

namespace NunyFoodWebApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<NunyFoodDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<NunyFoodDbContext>());

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddScoped<IPaymentProvider, FakePaymentProvider>();

        services.AddHostedService<DatabaseSeeder>();

        return services;
    }
}
