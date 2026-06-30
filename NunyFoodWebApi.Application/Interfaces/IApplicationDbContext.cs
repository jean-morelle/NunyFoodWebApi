using Microsoft.EntityFrameworkCore;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Admin> Admins { get; }
    DbSet<Customer> Customers { get; }
    DbSet<Beneficiary> Beneficiaries { get; }
    DbSet<Product> Products { get; }
    DbSet<Pack> Packs { get; }
    DbSet<PackProduct> PackProducts { get; }
    DbSet<Order> Orders { get; }
    DbSet<Payment> Payments { get; }
    DbSet<OrderStatusHistory> OrderStatusHistories { get; }
    DbSet<DeliveryAgent> DeliveryAgents { get; }
    DbSet<Delivery> Deliveries { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
