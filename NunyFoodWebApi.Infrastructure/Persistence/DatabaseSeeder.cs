using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Infrastructure.Persistence;

public class DatabaseSeeder(IServiceProvider serviceProvider, ILogger<DatabaseSeeder> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<NunyFoodDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        await db.Database.MigrateAsync(cancellationToken);

        if (!await db.Admins.AnyAsync(cancellationToken))
        {
            db.Admins.Add(new Admin
            {
                FullName = "Super Admin",
                Email = "koudorojeanmorelle408@gmail.com",
                PasswordHash = hasher.Hash("Admin@1234"),
            });
            await db.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Seeded default admin: koudorojeanmorelle408@gmail.com / Admin@1234");
        }

        if (!await db.DeliveryAgents.AnyAsync(cancellationToken))
        {
            db.DeliveryAgents.Add(new DeliveryAgent
            {
                FullName = "Agent par défaut",
                Email = "livreur@nunyfood.com",
                PasswordHash = hasher.Hash("Livreur@1234"),
                PhoneNumber = "+22890000001",
                Zone = "Lomé Centre",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
            });
            await db.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Seeded default agent: livreur@nunyfood.com / Livreur@1234");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
